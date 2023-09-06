using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Lottery.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Lottery.Domain.Database.Entity;
using Lottery.Service;

namespace Lottery.ViewModel;

public partial class DrawPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int choosen = 0;

    //[ObservableProperty]
    //private ObservableCollection<MyDrawableNumbers> numbers;

    [ObservableProperty]
    private ObservableCollection<MyDrawableNumber> shownNumbers;

    private List<int> drawnNumbers;

    private Random rand = new Random();

    [ObservableProperty]
    private string howMany = "";

    [ObservableProperty]
    private string communication = "";

    [ObservableProperty]
    private bool isButtonEnabled = true;

    private int drawnChoosen = 0;

    public DrawPageViewModel() 
    {
        drawnNumbers = new List<int>();
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

        Thread numberGenerator = new Thread(new ThreadStart(drawNumber));
        numberGenerator.Start();

        numberGenerator.Join();

        IsButtonEnabled = true;
    }

    private void drawNumber()
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

        //Numbers = new ObservableCollection<MyDrawableNumbers>();
        //ObservableCollection<MyDrawableNumbers> temp = new ObservableCollection<MyDrawableNumbers>();

        int number;

        //MyDrawableNumbers rowOfNumbers = new MyDrawableNumbers();

        drawnChoosen = Choosen;
        ShownNumbers = new ObservableCollection<MyDrawableNumber>();
        drawnNumbers.Clear();

        for (int e = 0; e < howMany; e++)
        {
            for (int i = 0; i < drawnChoosen; i++)
            {
                do
                {
                    number = rand.Next(drawnChoosen == 5 ? 91 : 46);
                } while (drawnNumbers.Contains(number));
                //rowOfNumbers.Append(new MyDrawableNumber(number, false));
                drawnNumbers.Add(number);
                ShownNumbers.Add(new MyDrawableNumber(number, false));
            }
            //temp.Add(rowOfNumbers);
            //rowOfNumbers = new MyDrawableNumbers();
        }
        
        //Numbers = temp;
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
        if (drawnNumbers.Count != 0)
        {
            Communication = "There are no numbers to be saved!";
            return;
        }

        /*
        List<int> saveableNumbers = new List<int>();

        foreach (MyDrawableNumbers numbers in Numbers)
        {
            saveableNumbers.Add(numbers.GetNumbers);
        }

        await DatabaseService.AddNumer(saveableNumbers, drawnChoosen);



        string numbersInString = "";

        foreach (MyDrawableNumbers numbers in Numbers)
        {
            
            for(int i = 0; i < numbers.GetNumbers().Count; i++)
            {
                numbersInString += (numbers.GetNumbers()[i].ToString() + ';');
            }
        }
        await DatabaseService.AddNumer(numbersInString, drawnChoosen);
        */
        Communication = "Save completed";
    }

    [RelayCommand]
    public async Task show()
    {
        //MyNumbersEntity mynumbers = await DatabaseService.GetLatestNumbers();
        //Communication = mynumbers.numbers;
    }
}

