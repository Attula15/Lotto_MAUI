using Lottery.ViewModel;

namespace Lottery.View;

public partial class DrawPage : ContentPage
{
	public DrawPage(DrawPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}