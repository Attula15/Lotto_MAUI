using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Lottery.Service;
using Lottery.POCO;
using Lottery.Domain;
using Lottery.View;

namespace Lottery.ViewModel;

public partial class DrawPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int choosen = 0;

    [ObservableProperty]
    private ObservableCollection<MyDrawableNumberPOCO> shownNumbers;

    //[ObservableProperty]
    //private GridItemsLayout collectionViewItemsLayout;

    private List<int> drawnNumbers;

    private Random rand = new Random();

    [ObservableProperty]
    private string howMany = "";

    [ObservableProperty]
    private string communication = "";

    [ObservableProperty]
    private bool isDrawButtonEnabled = true;

    [ObservableProperty]
    private bool isCollection5Visible = false;
    
    [ObservableProperty]
    private bool isCollection6Visible = false;

    private int drawnChoosen = 0;

    [ObservableProperty]
    private int currentPage = 0;

    [ObservableProperty]
    private bool isLoading = false;

    private readonly IRestAPI restAPI;

    private readonly IKeyCloakService keyCloakService;

    public DrawPageViewModel(IRestAPI restAPI, IKeyCloakService keyCloack) 
    {
        drawnNumbers = new List<int>();
        this.restAPI = restAPI;
        this.keyCloakService = keyCloack;
        //CollectionViewItemsLayout = new GridItemsLayout(5, ItemsLayoutOrientation.Vertical);
    }

    [RelayCommand]
    public void drawNumbers()
    {
        IsLoading = true;
        
        Communication = "";
        if (Choosen == 0)
        {
            Communication = "You must first choose a drawable number!";
            return;
        }

        if(!IsDrawButtonEnabled)
        {
            return;
        }

        IsDrawButtonEnabled = false;

        Thread numberGenerator = new Thread(new ThreadStart(drawNumberThread));
        numberGenerator.Start();

        numberGenerator.Join();

        IsDrawButtonEnabled = true;
        
        IsLoading = false;
    }

    private void drawNumberThread()
    {
        int howMany;
        try
        {
            howMany = int.Parse(HowMany);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Communication = "The number that you gave is not an actual number!";
            return;
        }

        int number;

        ShownNumbers = new ObservableCollection<MyDrawableNumberPOCO>();
        drawnChoosen = Choosen;

        if (drawnChoosen == 5)
        {
            IsCollection5Visible = true;
            IsCollection6Visible = false;
        }
        else
        {
            IsCollection6Visible = true;
            IsCollection5Visible = false;
        }
        
        drawnNumbers = new List<int>();

        for (int e = 0; e < howMany; e++)
        {
            List<int> ticket = new List<int>();
            for (int i = 0; i < drawnChoosen; i++)
            {
                do
                {
                    number = rand.Next(drawnChoosen == 5 ? 91 : 46);
                } while (ticket.Contains(number) || number == 0);
                drawnNumbers.Add(number);
                ticket.Add(number);
            }
        }
        for(int i = 0; i < drawnNumbers.Count; i++)
        {
            ShownNumbers.Add(new MyDrawableNumberPOCO(drawnNumbers[i], false));
        }
        CurrentPage = 1;
    }

    public void checkEntry(string newText)
    {
        Communication = "";
        IsDrawButtonEnabled = true;
        bool ok = false;
        try
        {
            if(int.Parse(newText) > 0)
            {
                ok = true;
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
        }
        if (!ok)
        {
            IsDrawButtonEnabled = false;
            Communication = "Number that you gave is not a valid number!";
        }
    }

    [RelayCommand]
    public async Task saveTheNumbers()
    {
        IsLoading = true;
        IsDrawButtonEnabled = false;

        Communication = "";
        if (drawnNumbers.Count == 0)
        {
            Communication = "There are no numbers to be saved!";
            return;
        }

        await DatabaseService.AddNumber(drawnNumbers, IsCollection5Visible ? 5 : 6, keyCloakService.GetCurrentUsername());
        bool success = await restAPI.uploadNumbers(drawnNumbers, IsCollection5Visible ? 5 : 6);

        if(success)
        {
            Communication = "Save completed";
        }
        else
        {
            Communication = "The server could not save the numbers";
        }

        IsLoading = false;
        IsDrawButtonEnabled = true;
    }

    [RelayCommand]
    private async void Logout()
    {
        bool success = await keyCloakService.Logout();

        if (success)
        {
            Debug.WriteLine("Logged out");
            App.Current.MainPage = new LoginPage(new LoginViewModel(keyCloakService));
        }
    }
}

