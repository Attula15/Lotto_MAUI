using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Service;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Lottery.ViewModel;
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string username = "";
    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string communication = "";

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isEnabled = true;

    private static string url = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/token";

    [RelayCommand]
    private async void Login()
    {
        IsLoading = true;
        IsEnabled = false;
        Communication = "";
        KeyCloakResponsePOCO responseJson;
        using HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        List<KeyValuePair<string, string>> bodyData = new List<KeyValuePair<string, string>>();

        bodyData.Add(new KeyValuePair<string, string>("client_id", "login-app"));
        bodyData.Add(new KeyValuePair<string, string>("username", Username));
        bodyData.Add(new KeyValuePair<string, string>("password", Password));
        bodyData.Add(new KeyValuePair<string, string>("grant_type", "password"));

        HttpContent content = new FormUrlEncodedContent(bodyData);
        HttpResponseMessage response = await client.PostAsync("", content);
        Debug.WriteLine("Username: " + Username + " Password: " + Password);
        Debug.WriteLine(response.Content);
        if (response.IsSuccessStatusCode)
        {
            responseJson = await response.Content.ReadFromJsonAsync<KeyCloakResponsePOCO>();
            await DatabaseService.AddToken(responseJson.access_token, responseJson.refresh_token);
            App.Current.MainPage = new AppShell();
        }
        else
        {
            Debug.WriteLine("Status code: " + response.StatusCode);
            Communication = "Wrong username or password!";
        }
        IsEnabled = true;
        IsLoading = false;
    }
}

