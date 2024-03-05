using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;

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

    private readonly IKeyCloakService keycloak;

    public LoginViewModel(IKeyCloakService keycloak)
    {
        this.keycloak = keycloak;
    }

    [RelayCommand]
    private async Task Login()
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

