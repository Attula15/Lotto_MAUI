using Lottery.ViewModel;

namespace Lottery.View;

public partial class SavedPage : ContentPage
{
	private SavedPageViewModel viewModel;
	public SavedPage(SavedPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		viewModel = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel.LoadNumbersIntoDrawnList();
    }
}