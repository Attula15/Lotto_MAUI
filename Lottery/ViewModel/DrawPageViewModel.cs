using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Lottery.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Lottery.Domain.Database.Entity;
using Lottery.Service;

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

        Numbers = new ObservableCollection<MyDrawableNumbers>();
        ObservableCollection<MyDrawableNumbers> temp = new ObservableCollection<MyDrawableNumbers>();

        int number;

        MyDrawableNumbers rowOfNumbers = new MyDrawableNumbers();

        for (int e = 0; e < howMany; e++)
        {
            for (int i = 0; i < Choosen; i++)
            {
                do
                {
                    Debug.WriteLine("Random number");
                    number = rand.Next(Choosen == 5 ? 91 : 46);
                } while (rowOfNumbers.Contains(new MyDrawableNumber(number, false)));
                rowOfNumbers.Append(new MyDrawableNumber(number, false));
            }
            temp.Add(rowOfNumbers);
            Debug.WriteLine("Random row");
            rowOfNumbers = new MyDrawableNumbers();
        }
        Numbers = temp;
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

    [RelayCommand]
    public async Task saveTheNumbers()
    {
        Communication = "";
        if (Numbers == null)
        {
            Communication = "There are no numbers to be saved!";
            return;
        }
        
        string numbersInString = "";

        foreach (MyDrawableNumbers numbers in Numbers)
        {
            
            for(int i = 0; i < numbers.GetNumbers().Count; i++)
            {
                Debug.WriteLine("Ez az egyik szám: " + numbers.GetNumbers()[i].ToString());
                numbersInString += (numbers.GetNumbers()[i].ToString() + ';');
            }
        }
        Debug.WriteLine("Ez pedig a végeleges" + numbersInString);
        await DatabaseService.AddNumer(numbersInString, Choosen);

        Communication = "Save completed";
    }

    [RelayCommand]
    public async Task show()
    {
        Debug.WriteLine("hello");
        MyNumbers mynumbers = await DatabaseService.GetLatestNumbers();
        Debug.WriteLine(mynumbers);
        Debug.WriteLine(mynumbers.date);
        Debug.WriteLine(mynumbers.numbers);
        Debug.WriteLine(mynumbers.numberType);
        Communication = mynumbers.numbers;
    }
}

