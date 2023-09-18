using Lottery.ViewModel;

namespace Lottery.View;

public partial class DataPage : ContentPage
{
    private readonly DataPageViewModel viewModel;

	public DataPage(DataPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.LoadData();
    }
}