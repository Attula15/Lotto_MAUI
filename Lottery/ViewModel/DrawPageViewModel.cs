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
        int howMany = 0;
        Numbers = new ObservableCollection<MyDrawableNumbers>();
        try
        {
            howMany = int.Parse(HowMany);
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
                int number;
                if(Choosen == 5)
                {
                    do
                    {
                        number = rand.Next(91);
                    } while (rowOfNumbers.Contains(new MyDrawableNumber(number, false)));
                }
                else if(Choosen == 6)
                {
                    do
                    {
                        number = rand.Next(46);
                    } while (rowOfNumbers.Contains(new MyDrawableNumber(number, false)));
                }
                else
                {
                    number = -1;
                }
                rowOfNumbers.Append(new MyDrawableNumber(number, false));
            }
            Numbers.Add(rowOfNumbers);
        }
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
            Communication = "Number that you gave is not a number!";
        }
    }
}

