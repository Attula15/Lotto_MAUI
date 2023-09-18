using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
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

    [ObservableProperty]
    private bool isPassword = true;

    [ObservableProperty]
    private bool isVisible = false;

    private static string url = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/token";

    private readonly IKeyCloakService keycloak;

    public LoginViewModel(IKeyCloakService keycloak)
    {
        this.keycloak = keycloak;
    }

    [RelayCommand]
    private async void Login()
    {
        IsLoading = true;
        IsEnabled = false;
        Communication = "";
        Communication = await keycloak.Login(Username, Password);
        IsEnabled = true;
        IsLoading = false;
    }

    [RelayCommand]
    private void showPassword()
    {
        IsVisible = !IsVisible;
        IsPassword = !IsPassword;
    }
}

