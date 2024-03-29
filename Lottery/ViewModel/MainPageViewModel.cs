﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.POCO;
using Lottery.View;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Lottery.Mapper;
using Lottery.Service;

namespace Lottery.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private bool visible = true;

    [ObservableProperty]
    private string prize5;
    
    [ObservableProperty]
    private string prize6;

    [ObservableProperty]
    private ObservableCollection<MyDrawableNumberPOCO> lottery5WinningNumbers;
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumberPOCO> lottery6WinningNumbers;

    [ObservableProperty]
    private bool isLoading = false;

    private readonly IRestAPI restAPI;

    private readonly IKeyCloakService keyCloak;

    private readonly CachingService cachingService;

    public MainPageViewModel(IRestAPI rest, IKeyCloakService keyCloak, CachingService cachingService)
    {
        restAPI = rest;
        this.keyCloak = keyCloak;
        this.cachingService = cachingService;
    }

    public async void Init()
    {
        IsLoading = true;
        await getPrizes();
        await GetWinningNumbers();
        IsLoading = false;
    }

    private async Task getPrizes()
    {
        PrizesHolderPOCO result = null;

        result = await cachingService.GetPrizes();
        
        if(result.prizes.Count != 0)
        {
            Debug.WriteLine("The result that I got: " + result.prizes[0].ToString() + ";" + result.prizes[1].ToString());
            List<PrizesPOCO> prizes = result.prizes;
            for (int i = 0; i < prizes.Count; i++)
            {
                if (prizes[i].whichOne.Equals(5))
                {
                    Prize5 = prizes[i].prize.ToString() + " million HUF";
                }
                if (prizes[i].whichOne.Equals(6))
                {
                    Prize6 = prizes[i].prize.ToString() + " million HUF";
                }
            }
        }
        else
        {
            Prize5 = "Could not connect to the server";
            Prize6 = "Could not connect to the server";
        }
    }

    [RelayCommand]
    public async Task OpenBrowser()
    {
        try
        {
            Uri uri = new Uri("https://www.szerencsejatek.hu/");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }

    private async Task GetWinningNumbers()
    {
        MyNumbersPOCO winning5 = await cachingService.GetWinningNumbers(5);
        MyNumbersPOCO winning6 = await cachingService.GetWinningNumbers(6);
        
        List<int> listOfNumbers;
        for (int i = 0; i < 2; i++)
        {
            if (i % 2 == 0)
            {
                listOfNumbers = winning5.numbers;
            }
            else
            {
                listOfNumbers = winning6.numbers;
            }

            ObservableCollection<MyDrawableNumberPOCO> temp = new ObservableCollection<MyDrawableNumberPOCO>();

            foreach (int number in listOfNumbers)
            {
                temp.Add(new MyDrawableNumberPOCO(number, false));
            }

            if (i % 2 == 0)
            {
                Lottery5WinningNumbers = temp;
            }
            else
            {
                Lottery6WinningNumbers = temp;
            }
        }
    }

    [RelayCommand]
    public async Task Logout()
    {
        bool success = await keyCloak.Logout();

        if(success)
        {
            Debug.WriteLine("Logged out");
            App.Current.MainPage = new LoginPage(new LoginViewModel(keyCloak));
        }
    }
}

