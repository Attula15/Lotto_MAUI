using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.Entity;
using System.Diagnostics;

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
    public async Task welcomeMessage()
    {
        Name = TempName;
        WinningNumbersEntity entity = await restAPI.GetWinningnumbers("5");
        Name = entity.numbers;
    }

    public async void getPrizes()
    {
        PrizesHolderEntity result = null;
        try
        {
            result = await restAPI.GetPrizes();
        }
        catch (Exception ex){
            Debug.WriteLine(ex);
        }
        if(result != null)
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
            Console.WriteLine("Error");
        }
    }
}

