﻿using CommunityToolkit.Mvvm.ComponentModel;
using Lottery.Domain;
using Lottery.Domain.ResponseBody;
using Lottery.POCO;
using Lottery.Service;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Lottery.ViewModel;
public partial class SavedPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumberPOCO> displayedLottery5Numbers = new ObservableCollection<MyDrawableNumberPOCO>();
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumberPOCO> displayedLottery6Numbers = new ObservableCollection<MyDrawableNumberPOCO>();

    [ObservableProperty]
    private bool isLoading = false;

    private readonly IRestAPI restAPI;
    public SavedPageViewModel(IRestAPI rest)
    {
        restAPI = rest;
    }

    public SavedPageViewModel() { }

    public async void LoadNumbersIntoDrawnList()
    {
        IsLoading = true;

        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumberPOCO>();
        DisplayedLottery6Numbers = new ObservableCollection<MyDrawableNumberPOCO>();

        MyNumbersPOCO lottery5NumbersFromAPI = MyNumberMapper.toPOCOFromSavedNumbersPOCO(await restAPI.getSavedNumbersFromAPI(5), 5);
        MyNumbersPOCO lottery6NumbersFromAPI = MyNumberMapper.toPOCOFromSavedNumbersPOCO(await restAPI.getSavedNumbersFromAPI(6), 6);

        MyNumbersPOCO lottery5Numbers = await DatabaseService.GetLatestNumbers(5);
        MyNumbersPOCO lottery6Numbers = await DatabaseService.GetLatestNumbers(6);

        MyNumbersPOCO useable5Nummbers = lottery5Numbers;
        MyNumbersPOCO useable6Nummbers = lottery6Numbers;

        if(lottery5NumbersFromAPI != null)
        {
            if(lottery5Numbers == null)
            {
                useable5Nummbers = lottery5NumbersFromAPI;
            }
            else
            {
                if (lottery5NumbersFromAPI.date > lottery5Numbers.date)
                {
                    useable5Nummbers = lottery5NumbersFromAPI;
                }
            }
        }

        if(lottery6NumbersFromAPI != null)
        {
            if (lottery6Numbers == null)
            {
                useable6Nummbers = lottery6NumbersFromAPI;
            }
            else
            {
                if (lottery6NumbersFromAPI.date > lottery6Numbers.date)
                {
                    useable6Nummbers = lottery6NumbersFromAPI;
                }
            }
        }

        ObservableCollection<MyDrawableNumberPOCO> temp5 = new ObservableCollection<MyDrawableNumberPOCO>();
        ObservableCollection<MyDrawableNumberPOCO> temp6 = new ObservableCollection<MyDrawableNumberPOCO>();

        if (useable5Nummbers != null)
        {
            for (int i = 0; i < useable5Nummbers.numbers.Count; i++)
            {
                temp5.Add(new MyDrawableNumberPOCO(useable5Nummbers.numbers[i], false));
            }
        }

        DisplayedLottery5Numbers = temp5;

        if (useable6Nummbers != null)
        {
            for (int i = 0; i < useable6Nummbers.numbers.Count; i++)
            {
                temp6.Add(new MyDrawableNumberPOCO(useable6Nummbers.numbers[i], false));
            }
        }

        DisplayedLottery6Numbers = temp6;

        IsLoading = false;
    }
}
