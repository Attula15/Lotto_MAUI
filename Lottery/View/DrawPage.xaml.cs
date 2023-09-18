using Lottery.ViewModel;

namespace Lottery.View;

public partial class DrawPage : ContentPage
{
	private DrawPageViewModel viewModel;
	public DrawPage(DrawPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        viewModel = vm;
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(viewModel != null && !e.NewTextValue.Equals(""))
        {
            viewModel.checkEntry(e.NewTextValue);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //viewModel.Disappear();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        //viewModel.Appear();
    }
}