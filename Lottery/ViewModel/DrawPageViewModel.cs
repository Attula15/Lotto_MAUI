using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Lottery.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Lottery.ViewModel;

[QueryProperty(nameof(Choosen), "Choosen")]
public partial class DrawPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int choosen = 0;

    [ObservableProperty]
    private ObservableCollection<MyDrawableNumbers> numbers;

    private Random rand = new Random();

    [ObservableProperty]
    private string howMany = "";

    [ObservableProperty]
    private string communication = "";

    [ObservableProperty]
    private bool isButtonEnabled = true;

    [RelayCommand]
    public void drawNumbers()
    {
        if(!IsButtonEnabled)
        {
            return;
        }

        IsButtonEnabled = false;

        int howMany = 0;
        Numbers = new ObservableCollection<MyDrawableNumbers>();
        try
        {
            howMany = int.Parse(HowMany);
            if(howMany > 15)
            {
                howMany = 15;
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            Communication = "The number that you gave is not an actual number!";
            return;
        }
        
        //Multiple rows
        for(int i = 0; i < howMany; i++)
        {
            MyDrawableNumbers rowOfNumbers = new MyDrawableNumbers();
            //A row of numbers
            for(int e = 0; e < Choosen; e++)
            {
                rowOfNumbers = drawNumber(Choosen);
            }
            Numbers.Add(rowOfNumbers);
        }

        IsButtonEnabled = true;
    }

    private MyDrawableNumbers drawNumber(int choosen)
    {
        MyDrawableNumbers rowOfNumbers = new MyDrawableNumbers();
        int number;
        for(int i = 0; i < choosen; i++)
        {
            do
            {
                number = rand.Next(choosen == 5 ? 91 : 46);
            } while (rowOfNumbers.Contains(new MyDrawableNumber(number, false)));
            rowOfNumbers.Append(new MyDrawableNumber(number, false));
        }
        return rowOfNumbers;
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
                ok = true;
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
}

