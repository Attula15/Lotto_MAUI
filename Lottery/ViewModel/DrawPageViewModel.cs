using CommunityToolkit.Mvvm.ComponentModel;

namespace Lottery.ViewModel;

[QueryProperty(nameof(Choosen), "Choosen")]
public partial class DrawPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int choosen;
}

