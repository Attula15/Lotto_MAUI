using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lottery.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private String name = "";

    [ObservableProperty]
    private String tempName = "";

    [ObservableProperty]
    private bool visible = true;

    [RelayCommand]
    public void welcomeMessage()
    {
        Name = TempName;
    }
}

