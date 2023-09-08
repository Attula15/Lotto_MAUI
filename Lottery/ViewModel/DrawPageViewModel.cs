using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Lottery.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Lottery.Service;
using Lottery.POCO;

namespace Lottery.ViewModel;

public partial class DrawPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int choosen = 0;

    [ObservableProperty]
    private ObservableCollection<MyDrawableNumber> shownNumbers;

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

    public DrawPageViewModel() 
    {
        drawnNumbers = new List<int>();
        //CollectionViewItemsLayout = new GridItemsLayout(5, ItemsLayoutOrientation.Vertical);
    }

    [RelayCommand]
    public void drawNumbers()
    {
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
            Console.WriteLine(ex);
            Communication = "The number that you gave is not an actual number!";
            return;
        }

        int number;

        ShownNumbers = new ObservableCollection<MyDrawableNumber>();
        drawnChoosen = Choosen;
        /*
        if(drawnChoosen == 5)
        {
            Debug.WriteLine("Itt az 5");
            CollectionViewItemsLayout = new GridItemsLayout(5, ItemsLayoutOrientation.Horizontal);
        }
        else
        {
            Debug.WriteLine("Itt a 6");
            CollectionViewItemsLayout = new GridItemsLayout(6, ItemsLayoutOrientation.Horizontal);
        }
        */

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

        //ObservableCollection<MyDrawableNumber> ShownNumbersTemp = new ObservableCollection<MyDrawableNumber>();
        drawnNumbers = new List<int>();

        for (int e = 0; e < howMany; e++)
        {
            List<int> ticket = new List<int>();
            for (int i = 0; i < drawnChoosen; i++)
            {
                do
                {
                    number = rand.Next(drawnChoosen == 5 ? 91 : 46);
                } while (ticket.Contains(number));
                drawnNumbers.Add(number);
                ticket.Add(number);
                //ShownNumbersTemp.Add(new MyDrawableNumber(number, false));
            }
        }
        for(int i = 0; i < drawnNumbers.Count; i++)
        {
            ShownNumbers.Add(new MyDrawableNumber(drawnNumbers[i], false));
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
        Communication = "";
        if (drawnNumbers.Count == 0)
        {
            Communication = "There are no numbers to be saved!";
            return;
        }

        await DatabaseService.AddNumber(drawnNumbers, IsCollection5Visible ? 5 : 6);

        Communication = "Save completed";
    }

    [RelayCommand]
    public async Task show()
    {
        MyNumbersPOCO mynumbers = await DatabaseService.GetLatestNumbers(drawnChoosen);
        if(mynumbers != null && mynumbers.numbers != null)
        {
            Communication = mynumbers.numbers.ToString();
        }
        else
        {
            Communication = "Database is empty";
        }
    }

    public void Disappear()
    {
        ShownNumbers = new ObservableCollection<MyDrawableNumber>();
        Communication = "";
    }

    public void Appear()
    {
        if(drawnNumbers.Count != 0)
        {
            for (int i = 0; i < drawnNumbers.Count; i++)
            {
                ShownNumbers.Add(new MyDrawableNumber(drawnNumbers[i], false));
            }
        }
    }
}

