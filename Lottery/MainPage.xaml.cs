﻿using Lottery.ViewModel;

namespace Lottery;

public partial class MainPage : ContentPage
{
	private readonly MainPageViewModel viewmodel;

	public MainPage(MainPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		viewmodel = vm;
	}

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
		viewmodel.getPrizes();
    }
}

