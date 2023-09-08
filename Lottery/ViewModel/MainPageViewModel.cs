using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.Model;
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
            Console.WriteLine("Error: " + ex);
        }
    }

    /*
    [RelayCommand]
    public async void GetMyNumbers()
    {
        MyNumbers MyNumbers = await DatabaseService.GetLatestNumbers();
        if(MyNumbers == null)
        {
            return;
        }
        string NumbersInString = MyNumbers.numbers;
        Debug.WriteLine("MyNumberType: " + MyNumbers.numberType);
        if (MyNumbers.numberType == 5)
        {
            string[] listOfNumbers = NumbersInString.Split(';');
            List<int> ints = new List<int>();
            
            foreach(string number in listOfNumbers){
                string cleanNumber = number.Replace(";", "");
                Debug.WriteLine(cleanNumber);
                if(cleanNumber != "")
                {
                    ints.Add(int.Parse(cleanNumber));
                }
            }

            ObservableCollection<MyDrawableNumbers> temp = new ObservableCollection<MyDrawableNumbers>();
            MyDrawableNumbers rowOfNumbers = new MyDrawableNumbers();
            for(int i = 0; i < ints.Count; i++)
            {
                rowOfNumbers.Append(new MyDrawableNumber(ints[i], false));
                if(rowOfNumbers.Size() % 5 == 0)
                {
                    temp.Add(rowOfNumbers);
                    rowOfNumbers = new MyDrawableNumbers();
                }
            }
            
            this.MyNumbers = temp;
            Debug.WriteLine("The listOfNumbers: " + listOfNumbers);
        }
    }*/

    private async Task GetWinningNumbers()
    {
        WinningNumbersEntity winning5 = await restAPI.GetWinningnumbers("5");
        WinningNumbersEntity winning6 = await restAPI.GetWinningnumbers("6");
        string[] listOfNumbers;
        for (int i = 0; i < 2; i++)
        {
            if (i % 2 == 0)
            {
                listOfNumbers = winning5.numbers.Split(';');
            }
            else
            {
                listOfNumbers = winning6.numbers.Split(';');
            }

            ObservableCollection<MyDrawableNumber> temp = new ObservableCollection<MyDrawableNumber>();

            foreach (string number in listOfNumbers)
            {
                string cleanNumber = number.Replace(";", "");
                cleanNumber = cleanNumber.Trim();
                if (cleanNumber != string.Empty)
                {
                    temp.Add(new MyDrawableNumber(int.Parse(cleanNumber), false));
                }
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

