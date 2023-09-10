using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.Model;
using Lottery.POCO;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
    private ObservableCollection<MyDrawableNumber> lottery5WinningNumbers;
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumber> lottery6WinningNumbers;

    [ObservableProperty]
    private bool isLoading = false;

    private readonly IRestAPI restAPI;

    public MainPageViewModel(IRestAPI rest)
    {
        restAPI = rest;
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
        PrizesHolderEntity result = null;

        result = await restAPI.GetPrizes();
        
        if(result.prizes.Count != 0)
        {
            List<PrizesEntity> prizes = result.prizes;
            for (int i = 0; i < prizes.Count; i++)
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
        MyNumbersPOCO winning5 = await restAPI.GetWinningnumbers("5");
        MyNumbersPOCO winning6 = await restAPI.GetWinningnumbers("6");
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

            ObservableCollection<MyDrawableNumber> temp = new ObservableCollection<MyDrawableNumber>();

            foreach (int number in listOfNumbers)
            {
                temp.Add(new MyDrawableNumber(number, false));
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
}

