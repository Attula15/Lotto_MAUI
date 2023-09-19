using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.ResponseBody;
using Lottery.POCO;
using Lottery.Service;
using Lottery.View;
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
    private readonly IKeyCloakService keyCloakService;
    public SavedPageViewModel(IRestAPI rest, IKeyCloakService keyCloakService)
    {
        restAPI = rest;
        this.keyCloakService = keyCloakService;
    }

    public SavedPageViewModel() { }

    public async void LoadNumbersIntoDrawnList()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        IsLoading = true;

        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumberPOCO>();
        DisplayedLottery6Numbers = new ObservableCollection<MyDrawableNumberPOCO>();

        MyNumbersPOCO lottery5NumbersFromAPI = null;
        MyNumbersPOCO lottery6NumbersFromAPI = null;

        if (accessType == Connectivity.Current.NetworkAccess)
        {
            lottery5NumbersFromAPI = MyNumberMapper.toPOCOFromSavedNumbersPOCO(await restAPI.getSavedNumbersFromAPI(5), 5);
            lottery6NumbersFromAPI = MyNumberMapper.toPOCOFromSavedNumbersPOCO(await restAPI.getSavedNumbersFromAPI(6), 6);
        }

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
        
        MyNumbersPOCO winning5 = await restAPI.GetWinningnumbers(5);
        MyNumbersPOCO winning6 = await restAPI.GetWinningnumbers(6);

        if (useable5Nummbers != null)
        {
            foreach (var number in useable5Nummbers.numbers)
            {
                if (winning5.numberType != null && winning5.numbers.Contains(number))
                {
                    temp5.Add(new MyDrawableNumberPOCO(number, true));
                }
                else
                {
                    temp5.Add(new MyDrawableNumberPOCO(number, false));
                }
            }
        }

        DisplayedLottery5Numbers = temp5;

        if (useable6Nummbers != null)
        {
            foreach (var number in useable6Nummbers.numbers)
            {
                if (winning6.numberType != null && winning6.numbers.Contains(number))
                {
                    temp6.Add(new MyDrawableNumberPOCO(number, true));
                }
                else
                {
                    temp6.Add(new MyDrawableNumberPOCO(number, false));
                }
            }
        }

        DisplayedLottery6Numbers = temp6;

        IsLoading = false;
    }

    [RelayCommand]
    public async void Logout()
    {
        bool success = await keyCloakService.Logout();

        if (success)
        {
            Debug.WriteLine("Logged out");
            App.Current.MainPage = new LoginPage(new LoginViewModel(keyCloakService));
        }
    }
}
