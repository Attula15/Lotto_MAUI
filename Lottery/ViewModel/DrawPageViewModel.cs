﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Lottery.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Lottery.Service;
using Lottery.Domain.Database.Entity;
using SQLitePCL;

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
    private bool prevButtonEnabled = false;
    [ObservableProperty]
    private bool nextButtonEnabled = false;

    [ObservableProperty]
    private int currentPage = 0;
    private int maxNumberOfElements = 0;

    private static int NUMBER_OF_NUMBERS_IN_LOTTERY5 = 15;
    private static int NUMBER_OF_NUMBERS_IN_LOTTERY6 = 18;

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
        NextButtonEnabled = true;
        PrevButtonEnabled = false;
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
        for(int i = 0; i < Math.Min(drawnChoosen == 5 ? NUMBER_OF_NUMBERS_IN_LOTTERY5 : NUMBER_OF_NUMBERS_IN_LOTTERY6, howMany*drawnChoosen);i++)
        {
            ShownNumbers.Add(new MyDrawableNumber(drawnNumbers[i], false));
        }
        Debug.WriteLine("Number of elements: " + ShownNumbers.Count);
        Debug.WriteLine("Collection 5: " + IsCollection5Visible + " Collection 6: " + IsCollection6Visible);
        maxNumberOfElements = drawnNumbers.Count;
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

        await DatabaseService.AddNumber(drawnNumbers, drawnChoosen);

        Communication = "Save completed";
    }

    [RelayCommand]
    public async Task show()
    {
        MyNumbersEntity mynumbers = await DatabaseService.GetLatestNumbers(drawnChoosen);
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
            int numberOfNumbers = drawnChoosen == 5 ? NUMBER_OF_NUMBERS_IN_LOTTERY5 : NUMBER_OF_NUMBERS_IN_LOTTERY6;
            for (int i = (CurrentPage - 1) * numberOfNumbers; i < Math.Min(CurrentPage * numberOfNumbers, maxNumberOfElements); i++)
            {
                ShownNumbers.Add(new MyDrawableNumber(drawnNumbers[i], false));
            }
        }
    }

    [RelayCommand]
    private void NextPage()
    {
        int numberOfNumbers = drawnChoosen == 5 ? NUMBER_OF_NUMBERS_IN_LOTTERY5 : NUMBER_OF_NUMBERS_IN_LOTTERY6;

        ShownNumbers = new ObservableCollection<MyDrawableNumber>();

        for (int i = CurrentPage * numberOfNumbers; i < Math.Min((CurrentPage + 1) * numberOfNumbers, maxNumberOfElements); i++)
        {
            ShownNumbers.Add(new MyDrawableNumber(drawnNumbers[i], false));
        }

        PrevButtonEnabled = true;
        CurrentPage++;
        
        if (maxNumberOfElements <= CurrentPage * numberOfNumbers)
        {
            NextButtonEnabled = false;
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        int numberOfNumbers = drawnChoosen == 5 ? NUMBER_OF_NUMBERS_IN_LOTTERY5 : NUMBER_OF_NUMBERS_IN_LOTTERY6;
        
        ShownNumbers = new ObservableCollection<MyDrawableNumber>();                                            

        for (int i = (CurrentPage - 2) * numberOfNumbers; i < (CurrentPage - 1) * numberOfNumbers; i++)
        {
            ShownNumbers.Add(new MyDrawableNumber(drawnNumbers[i], false));
        }

        Debug.WriteLine("Start of i: " + (CurrentPage - 1) * numberOfNumbers);
        Debug.WriteLine("End of i: " + CurrentPage * numberOfNumbers);

        NextButtonEnabled = true;
        CurrentPage--;
        if (CurrentPage == 1)
        {
            PrevButtonEnabled = false;
        }
    }
}

