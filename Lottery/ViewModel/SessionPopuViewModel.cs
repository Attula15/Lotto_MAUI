
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.View;
using Mopups.Interfaces;
using System.Diagnostics;

namespace Lottery.ViewModel;
public partial class SessionPopuViewModel : ObservableObject
{
    private readonly IKeyCloakService keyCloakService;
    private readonly IPopupNavigation navigation;

    public SessionPopuViewModel(IKeyCloakService keyCloak, IPopupNavigation navigation)
    {
        keyCloakService = keyCloak;
        this.navigation = navigation;
    }

    [RelayCommand]
    private async void Yes()
    {
        Debug.WriteLine("Great, renewing...");
        await keyCloakService.RefreshToken();
        await navigation.PopAsync();
    }

    [RelayCommand]
    private async void No()
    {
        bool success = await keyCloakService.Logout();
        if (success)
        {
            Debug.WriteLine("Logged out");
            App.Current.MainPage = new LoginPage(new LoginViewModel(keyCloakService));
        }
        await navigation.PopAsync();
    }
}