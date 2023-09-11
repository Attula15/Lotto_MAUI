using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Net.Http.Json;
using Windows.Security.Authentication.Web.Core;
using Windows.System.UserProfile;
using static System.Net.WebRequestMethods;

namespace Lottery.ViewModel;
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string username = "";
    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string communication = "";

    private static string url = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/token";

    [RelayCommand]
    private async void Login()
    {
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
        if(response.IsSuccessStatusCode)
        {
            responseJson = await response.Content.ReadFromJsonAsync<KeyCloakResponsePOCO>();
            KeyCloakResponsePOCO KeyCloakResponseGLOBAL = responseJson;
            App.Current.MainPage = new AppShell();
        }
        else
        {
            Communication = "Wrong username or password!";
        }
    }
}

