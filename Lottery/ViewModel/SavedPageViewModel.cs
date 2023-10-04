using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
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
        IsLoading = true;

        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumberPOCO>();
        DisplayedLottery6Numbers = new ObservableCollection<MyDrawableNumberPOCO>();

        MyNumbersPOCO lottery5NumbersFromAPI = null;
        MyNumbersPOCO lottery6NumbersFromAPI = null;

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            lottery5NumbersFromAPI = MyNumberMapper.toPOCOFromSavedNumbersPOCO(await restAPI.getSavedNumbersFromAPI(5), 5);
            lottery6NumbersFromAPI = MyNumberMapper.toPOCOFromSavedNumbersPOCO(await restAPI.getSavedNumbersFromAPI(6), 6);
        }

        MyNumbersPOCO lottery5Numbers = await DatabaseService.GetLatestNumbers(5, keyCloakService.GetCurrentUsername());
        MyNumbersPOCO lottery6Numbers = await DatabaseService.GetLatestNumbers(6, keyCloakService.GetCurrentUsername());

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
                else
                {
                    if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                    {
                        await restAPI.uploadNumbers(useable5Nummbers.numbers, 5);   
                    }
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
                else
                {
                    if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                    {
                        await restAPI.uploadNumbers(useable6Nummbers.numbers, 6);   
                    }
                }
            }
        }

        ObservableCollection<MyDrawableNumberPOCO> temp5 = new ObservableCollection<MyDrawableNumberPOCO>();
        ObservableCollection<MyDrawableNumberPOCO> temp6 = new ObservableCollection<MyDrawableNumberPOCO>();
        
        MyNumbersPOCO winning5 = await restAPI.GetWinningnumbers(5);
        MyNumbersPOCO winning6 = await restAPI.GetWinningnumbers(6);

        TimeSpan oneWeek = new TimeSpan(7, 0, 0, 0);
        
        if (useable5Nummbers != null)
        {

            DateTime? winning5DateTime;

            try
            {
                //Azert kell, mert csak eddig lehet szelvenyt feladni
                winning5DateTime = new DateTime(winning5.date.Value.Year, winning5.date.Value.Month, winning5.date.Value.Day, 17, 30, 0);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                winning5DateTime = new DateTime(1900, 1, 1);
            }
            
            bool ticketTooOld = winning5DateTime - useable5Nummbers.date > oneWeek;
            
            if (winning5.date > useable5Nummbers.date && !ticketTooOld)
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
            else
            {
                foreach (var number in useable5Nummbers.numbers)
                {
                    temp5.Add(new MyDrawableNumberPOCO(number, false));
                }
            }
            
        }

        DisplayedLottery5Numbers = temp5;

        if (useable6Nummbers != null)
        {
            DateTime? winning6DateTime;
            
            try
            {
                //Azert kell, mert csak eddig lehet szelvenyt feladni
                winning6DateTime = new DateTime(winning5.date.Value.Year, winning5.date.Value.Month, winning5.date.Value.Day, 14, 30, 0);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                winning6DateTime = new DateTime(1900, 1, 1);
            }
            
            bool ticketTooOld = winning6DateTime - useable6Nummbers.date > oneWeek;
            
            if (winning6.date > useable6Nummbers.date && !ticketTooOld)
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
            else
            {
                foreach (var number in useable6Nummbers.numbers)
                {
                    temp6.Add(new MyDrawableNumberPOCO(number, false));
                }
            }
        }

        DisplayedLottery6Numbers = temp6;

        IsLoading = false;
    }

    [RelayCommand]
    public async Task Logout()
    {
        bool success = await keyCloakService.Logout();

        if (success)
        {
            Debug.WriteLine("Logged out");
            App.Current.MainPage = new LoginPage(new LoginViewModel(keyCloakService));
        }
    }
}
