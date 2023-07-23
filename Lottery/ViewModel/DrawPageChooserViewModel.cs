using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.View;

namespace Lottery.ViewModel;
public partial class DrawPageChooserViewModel : ObservableObject
{
    [ObservableProperty]
    private string value;

    [ObservableProperty]    
    private string message;

    [ObservableProperty]
    private bool messageVisible = false;

    [RelayCommand]
    private void ConfirmSelection()
    {
        MessageVisible = false;
        if(Value == null)
        {
            Message = "You have not selected any options, please select one";
            MessageVisible = true;
        }
        Shell.Current.GoToAsync($"{nameof(DrawPage)}?Choosen={Value}");
    }
}

