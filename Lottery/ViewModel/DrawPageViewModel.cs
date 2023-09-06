using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Lottery.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Lottery.Service;
using Lottery.Domain.Database.Entity;

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
    private bool isButtonEnabled = true;

    [ObservableProperty]
    private bool isCollection5Visible = false;
    
    [ObservableProperty]
    private bool isCollection6Visible = false;

    private int drawnChoosen = 0;

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

        if(!IsButtonEnabled)
        {
            return;
        }

        IsButtonEnabled = false;

        Thread numberGenerator = new Thread(new ThreadStart(drawNumberThread));
        numberGenerator.Start();

        numberGenerator.Join();

        IsButtonEnabled = true;
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
        Debug.WriteLine("Choosen: " + Choosen);
        Debug.WriteLine("drawChoosen: " + drawnChoosen);

        if (drawnChoosen == 5)
        {
            Debug.WriteLine("Itt az 5");
            IsCollection5Visible = true;
            IsCollection6Visible = false;
        }
        else
        {
            Debug.WriteLine("Itt a 6");
            IsCollection6Visible = true;
            IsCollection5Visible = false;
        }

        //ObservableCollection<MyDrawableNumber> ShownNumbersTemp = new ObservableCollection<MyDrawableNumber>();
        drawnNumbers = new List<int>();

        for (int e = 0; e < howMany; e++)
        {
            for (int i = 0; i < drawnChoosen; i++)
            {
                do
                {
                    number = rand.Next(drawnChoosen == 5 ? 91 : 46);
                } while (drawnNumbers.Contains(number));
                drawnNumbers.Add(number);
                //ShownNumbersTemp.Add(new MyDrawableNumber(number, false));
            }
        }
        for(int i = 0; i < Math.Min(drawnChoosen == 5 ? 25 : 30, howMany*drawnChoosen);i++)
        {
            ShownNumbers.Add(new MyDrawableNumber(drawnNumbers[i], false));
        }
        Debug.WriteLine("Number of elements: " + ShownNumbers.Count);
        Debug.WriteLine("Collection 5: " + IsCollection5Visible + " Collection 6: " + IsCollection6Visible);
        IsButtonEnabled = true;
    }

    public void checkEntry(string newText)
    {
        Communication = "";
        IsButtonEnabled = true;
        bool ok = false;
        try
        {
            if(int.Parse(newText) > 0)
            {
                if(int.Parse(newText) > 50)
                {
                    Communication = "The number you choose is too high! The maximum number is 50";
                }
                else
                {
                    ok = true;
                }
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
        }
        if (!ok)
        {
            IsButtonEnabled = false;
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

        await DatabaseService.AddNumber(drawnNumbers, drawnChoosen);

        Communication = "Save completed";
    }

    [RelayCommand]
    public async Task show()
    {
        MyNumbersEntity mynumbers = await DatabaseService.GetLatestNumbers(drawnChoosen);
        Communication = mynumbers.numbers.ToString();
    }
}

