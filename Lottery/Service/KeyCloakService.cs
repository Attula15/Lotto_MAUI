using System.Diagnostics;
using System.Net.Http.Json;
using Lottery.Domain;

namespace Lottery.Service;
public class KeyCloakService : IKeyCloakService
{
    private static string url = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/token";
    private static string logoutURL = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/logout";
    private static string sessionToken { get; set; }
    private static string refreshToken { get; set; }
    private static DateTime expireDate { get; set; }

    public string GetRefreshToken()
    {
        return refreshToken;
    }

    public string GetSessionToken()
    {
        return sessionToken;
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
        HttpResponseMessage response = await client.PostAsync("", content);
        if (response.IsSuccessStatusCode)
        {
            responseJson = await response.Content.ReadFromJsonAsync<KeyCloakResponsePOCO>();
            //await DatabaseService.AddToken(responseJson.access_token, responseJson.refresh_token);
            sessionToken = responseJson.access_token;
            refreshToken = responseJson.refresh_token;
            expireDate = DateTime.Now.AddSeconds(responseJson.expires_in);
            App.Current.MainPage = new AppShell();
            return "";
        }
        else
        {
            Debug.WriteLine("Status code: " + response.StatusCode);
            return "Wrong username or password!";
        }
    }

    public async Task<bool> Logout()
    {
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
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return false;
        }

        return true;
    }
}
