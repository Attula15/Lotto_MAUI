﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.Entity;

namespace Lottery.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private string tempName = "";

    [ObservableProperty]
    private bool visible = true;

    [ObservableProperty]
    private string prize5;
    
    [ObservableProperty]
    private string prize6;

    private readonly IRestAPI restAPI;

    public MainPageViewModel(IRestAPI rest)
    {
        restAPI = rest;
    }

    [RelayCommand]
    public async void welcomeMessage()
    {
        Name = TempName;
        WinningNumbersEntity entity = await restAPI.GetWinningnumbers("5");
        Name = entity.numbers;
    }

    public async void getPrizes()
    {
        PrizesHolderEntity result = await restAPI.GetPrizes();
        List<PrizesEntity> prizes = result.prizes;
        for(int i = 0; i < prizes.Count; i++)
        {
            if (prizes[i].whichOne.Equals("5"))
            {
                Prize5 = prizes[i].prize.ToString() + " million HUF";
            }
            if (prizes[i].whichOne.Equals("6"))
            {
                Prize6 = prizes[i].prize.ToString() + " million HUF";
            }
        }
    }
}

