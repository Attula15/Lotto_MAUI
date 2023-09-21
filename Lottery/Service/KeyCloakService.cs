using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using Lottery.Domain;
using Lottery.HelperView;
using Lottery.ViewModel;
using Mopups.Interfaces;
using Mopups.Services;

namespace Lottery.Service;
public class KeyCloakService : IKeyCloakService
{
    private static string url = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/token";
    private static string logoutURL = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/logout";
    private static string sessionToken { get; set; }
    private static string refreshToken { get; set; }
    private static DateTime? expireDate { get; set; }
    private static Thread sessionHandler { get; set; }
    private static bool loggedIn { get; set; } = false;

    private readonly IPopupNavigation popupNavigation;

    public string GetRefreshToken()
    {
        return refreshToken;
    }

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
        KeyCloakResponsePOCO responseJson;
        using HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        List<KeyValuePair<string, string>> bodyData = new List<KeyValuePair<string, string>>();

        bodyData.Add(new KeyValuePair<string, string>("client_id", "login-app"));
        bodyData.Add(new KeyValuePair<string, string>("username", username));
        bodyData.Add(new KeyValuePair<string, string>("password", password));
        bodyData.Add(new KeyValuePair<string, string>("grant_type", "password"));

        HttpContent content = new FormUrlEncodedContent(bodyData);
        try
        {
            HttpResponseMessage response = await client.PostAsync("", content);
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
                return "";
            }

            Debug.WriteLine("Status code: " + response.StatusCode);
            return "Wrong username or password!";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
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
