using Lottery.ViewModel;

namespace Lottery.View;

public partial class SavedPage : ContentPage
{
	public SavedPage(SavedPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}