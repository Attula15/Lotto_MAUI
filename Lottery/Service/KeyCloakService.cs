﻿using System.Diagnostics;
using System.Net.Http.Json;
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
        HttpResponseMessage response = await client.PostAsync("", content);
        if (response.IsSuccessStatusCode)
        {
            responseJson = await response.Content.ReadFromJsonAsync<KeyCloakResponsePOCO>();
            //await DatabaseService.AddToken(responseJson.access_token, responseJson.refresh_token);
            sessionToken = responseJson.access_token;
            refreshToken = responseJson.refresh_token;
            expireDate = DateTime.Now.AddSeconds(responseJson.expires_in);
            App.Current.MainPage = new AppShell();
            Thread sessionHandler = new Thread(new ThreadStart(Refresh));
            sessionHandler.Start();
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
            sessionToken = "";
            refreshToken = "";
            expireDate = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return false;
        }

        return true;
    }

    public async void Refresh()
    {
        Debug.WriteLine("10 Seconds started");
        Thread.Sleep(10000);
        Debug.WriteLine("10 Seconds ended");
        await popupNavigation.PushAsync(new SessionPopup(new SessionPopuViewModel(this, popupNavigation)));
    }
}
