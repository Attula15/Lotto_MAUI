using System.Diagnostics;
using System.Net.Http.Json;
using Lottery.Domain;
using Lottery.Domain.Database.Entity;
using Lottery.HelperView;
using Lottery.POCO;
using Lottery.ViewModel;
using Mopups.Interfaces;

namespace Lottery.Service;
public class KeyCloakService : IKeyCloakService
{
    private static string url = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/token";
    private static string logoutURL = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/logout";
    private static string introspectURL = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/token/introspect";

    private static string client_secret = "iLHJrpzCf0R8lFUdernRhZUyDxQSbw82";
    private static string sessionToken { get; set; }
    private static string refreshToken { get; set; }
    private static DateTime? expireDate { get; set; }
    private static Thread sessionHandler { get; set; }
    private static bool loggedIn { get; set; } = false;

    private readonly IPopupNavigation popupNavigation;

    public string GetSessionToken()
    {
        return sessionToken;
    }

    public KeyCloakService(IPopupNavigation popup)
    {
        this.popupNavigation = popup;
    }

    public async Task<string> Login(string username, string password)
    {
        using HttpClient client = new HttpClient();
        KeyCloakResponsePOCO responseJson;
        List<KeyValuePair<string, string>> bodyData = new List<KeyValuePair<string, string>>();
        HttpContent content;
        
        TokenEntity token = await DatabaseService.GetToken();
        
        //Get the token from the DB, if it exists
        if (token != null && token.username != null && token.username.Equals(username))
        {
            Debug.WriteLine("The token is not null");
            client.BaseAddress = new Uri(introspectURL);

            bodyData.Add(new KeyValuePair<string, string>("client_id", "login-app"));
            bodyData.Add(new KeyValuePair<string, string>("token", token.access_token));
            bodyData.Add(new KeyValuePair<string, string>("client_secret", client_secret));

            content = new FormUrlEncodedContent(bodyData);

            try
            {
                HttpResponseMessage response = await client.PostAsync("", content);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("The response is OK");
                    KeyCloakIntrospectResponsePOCO responseIntrospectJson = await response.Content.ReadFromJsonAsync<KeyCloakIntrospectResponsePOCO>();
                    if (responseIntrospectJson.active)
                    {
                        Debug.WriteLine("The user is active");
                        sessionToken = token.access_token;
                        refreshToken = token.refresh_token;
                        await RefreshToken();
                        Debug.WriteLine("Refresh has been called");
                        App.Current.MainPage = new AppShell();
                        Debug.WriteLine("Main page altered");
                        loggedIn = true;
                        sessionHandler = new Thread(RefreshMopupCaller);
                        sessionHandler.Name = "SessionHandler Thread";
                        sessionHandler.Start();
                        Debug.WriteLine("Session thread started");
                        
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Could not connect to the server";
            }
        }
        
        //If token cannot be used, then get one
        using HttpClient client2 = new HttpClient();
        client2.BaseAddress = new Uri(url);

        bodyData = new List<KeyValuePair<string, string>>();
        
        bodyData.Add(new KeyValuePair<string, string>("client_id", "login-app"));
        bodyData.Add(new KeyValuePair<string, string>("username", username));
        bodyData.Add(new KeyValuePair<string, string>("password", password));
        bodyData.Add(new KeyValuePair<string, string>("grant_type", "password"));
        bodyData.Add(new KeyValuePair<string, string>("client_secret", client_secret));

        content = new FormUrlEncodedContent(bodyData);
        try
        {
            HttpResponseMessage response = await client2.PostAsync("", content);
            if (response.IsSuccessStatusCode)
            {
                responseJson = await response.Content.ReadFromJsonAsync<KeyCloakResponsePOCO>();
                sessionToken = responseJson.access_token;
                refreshToken = responseJson.refresh_token;
                expireDate = DateTime.Now.AddSeconds(responseJson.expires_in);
                App.Current.MainPage = new AppShell();
                loggedIn = true;
                sessionHandler = new Thread(RefreshMopupCaller);
                sessionHandler.Name = "SessionHandler Thread";
                sessionHandler.Start();
                await DatabaseService.AddTokens(sessionToken, refreshToken, username);
                return "";
            }

            Debug.WriteLine("Status code: " + response.StatusCode);
            return "Wrong username or password!";
        }
        catch (Exception ex)
        {
            return "Could not connect to the server";
        }
    }

    public async Task<bool> Logout()
    {
        loggedIn = false;
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        try
        {
            List<KeyValuePair<string, string>> bodyData = new List<KeyValuePair<string, string>>();

            bodyData.Add(new KeyValuePair<string, string>("client_id", "login-app"));
            bodyData.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
            bodyData.Add(new KeyValuePair<string, string>("client_secret", client_secret));

            HttpContent content = new FormUrlEncodedContent(bodyData);
            HttpResponseMessage response = await client.PostAsync(logoutURL, content);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            sessionToken = "";
            refreshToken = "";
            expireDate = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            loggedIn = true;
            return false;
        }

        return true;
    }

    public async Task RefreshToken()
    {
        //Refresh
        loggedIn = false;
        KeyCloakResponsePOCO responseJson;
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        try
        {
            List<KeyValuePair<string, string>> bodyData = new List<KeyValuePair<string, string>>();

            bodyData.Add(new KeyValuePair<string, string>("client_id", "login-app"));
            bodyData.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            bodyData.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
            bodyData.Add(new KeyValuePair<string, string>("client_secret", client_secret));
            
            HttpContent content = new FormUrlEncodedContent(bodyData);
            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                responseJson = await response.Content.ReadFromJsonAsync<KeyCloakResponsePOCO>();
                refreshToken = responseJson.refresh_token;
                sessionToken = responseJson.access_token;
                Debug.WriteLine("New session token: " + sessionToken);
                expireDate = DateTime.Now.AddSeconds(responseJson.expires_in); 
                
                loggedIn = true;
                sessionHandler = new Thread(new ThreadStart(RefreshMopupCaller));
                sessionHandler.Name = "SessionHandler Thread";
                sessionHandler.Start();
                var token = await DatabaseService.GetToken();
                await DatabaseService.AddTokens(sessionToken, refreshToken, token.username);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async void RefreshMopupCaller()
    {
        Debug.WriteLine("Thread started");
        Debug.WriteLine("Milliseconds till expireDate" + expireDate.Value.Subtract(DateTime.Now).TotalMilliseconds);
        Debug.WriteLine("Seconds till expireDate" + expireDate.Value.Subtract(DateTime.Now).TotalSeconds);
        int seconds = (int) Math.Floor(expireDate.Value.Subtract(DateTime.Now).TotalSeconds);
        Debug.WriteLine("Seconds that gets into the thread" + seconds);
        Thread.Sleep((seconds - 60) * 1000);
        Debug.WriteLine("Thread wait ended");
        if(loggedIn)
        {
            await popupNavigation.PushAsync(new SessionPopup(new SessionPopuViewModel(this, popupNavigation)));
        }
        else
        {
            Debug.WriteLine("The user has already logged out");
        }
    }
}
