using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain.Database.Entity;
using Lottery.Model;
using Lottery.Service;
using System.Collections.ObjectModel;

namespace Lottery.ViewModel;
public partial class SavedPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumber> displayedLottery5Numbers = new ObservableCollection<MyDrawableNumber>();
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumber> displayedLottery6Numbers = new ObservableCollection<MyDrawableNumber>();

    [ObservableProperty]
    private bool isPrev5Enabled = false;
    [ObservableProperty]
    private bool isNext5Enabled = true;

    private List<int> drawnLottery5Numbers = new List<int>();

    private List<int> drawnLottery6Numbers = new List<int>();

    private static int NUMBER_OF_NUMBERS_IN_LOTTERY5 = 15;
    private static int NUMBER_OF_NUMBERS_IN_LOTTERY6 = 18;

    public async void LoadNumbersIntoDrawnList()
    {
        MyNumbersEntity lottery5Numbers = await DatabaseService.GetLatestNumbers(5);
        MyNumbersEntity lottery6Numbers = await DatabaseService.GetLatestNumbers(6);

        string[] listOf5NumbersInString = lottery5Numbers.numbers.Split(";");
        string[] listOf6NumbersInString = lottery6Numbers.numbers.Split(";");

        foreach(string number in listOf5NumbersInString)
        {
            drawnLottery5Numbers.Add(int.Parse(number.Replace(";", "").Replace("#", "").Trim()));
        }

        foreach (string number in listOf6NumbersInString)
        {
            drawnLottery6Numbers.Add(int.Parse(number.Replace(";", "").Replace("#", "").Trim()));
        }

        if(drawnLottery5Numbers.Count > 0)
        {
            for(int i = 0; i < Math.Min(NUMBER_OF_NUMBERS_IN_LOTTERY5, drawnLottery5Numbers.Count); i++)
            {
                DisplayedLottery5Numbers.Add(new MyDrawableNumber(drawnLottery5Numbers[i], false));
            }
        }

        if(drawnLottery6Numbers.Count > 0)
        {
            for (int i = 0; i < Math.Min(NUMBER_OF_NUMBERS_IN_LOTTERY6, drawnLottery6Numbers.Count); i++)
            {
                DisplayedLottery6Numbers.Add(new MyDrawableNumber(drawnLottery6Numbers[i], false));
            }
        }
    }

    [RelayCommand]
    public void NextPage5()
    {

    }

    [RelayCommand]
    public void PrevPage5()
    {

    }
}
