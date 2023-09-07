using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain.Database;
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
    private bool isNext5Enabled = false;

    [ObservableProperty]
    private bool isPrev6Enabled = false;
    [ObservableProperty]
    private bool isNext6Enabled = false;

    private static int NUMBER_OF_NUMBERS_IN_LOTTERY5 = 15;
    private static int NUMBER_OF_NUMBERS_IN_LOTTERY6 = 18;

    [ObservableProperty]
    private int currentPage5 = 0;
    [ObservableProperty]
    private int currentPage6 = 0;

    private int maxNumberOfElements5 = -1;
    private int maxNumberOfElements6 = -1;

    public async void LoadNumbersIntoDrawnList()
    {
        PageableNumbers lottery5Numbers = await DatabaseService.GetLatestPageableNumbers(5, 1);
        PageableNumbers lottery6Numbers = await DatabaseService.GetLatestPageableNumbers(6, 1);

        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumber>();
        DisplayedLottery6Numbers = new ObservableCollection<MyDrawableNumber>();

        if(lottery5Numbers != null)
        {
            for (int i = 0; i < lottery5Numbers.GetAllNumbersList().Count; i++)
            {
                DisplayedLottery5Numbers.Add(new MyDrawableNumber(lottery5Numbers.GetAllNumbersList()[i], false));
            }
            CurrentPage5 = 1;
            maxNumberOfElements5 = lottery5Numbers.MaxNumberOfElements;

            if (maxNumberOfElements5 <= NUMBER_OF_NUMBERS_IN_LOTTERY5)
            {
                IsNext5Enabled = false;
            }
            else
            {
                IsNext5Enabled = true;
            }
        }

        if (lottery6Numbers != null)
        {
            for (int i = 0; i < lottery6Numbers.GetAllNumbersList().Count; i++)
            {
                DisplayedLottery6Numbers.Add(new MyDrawableNumber(lottery6Numbers.GetAllNumbersList()[i], false));
            }
            CurrentPage6 = 1;
            maxNumberOfElements6 = lottery6Numbers.MaxNumberOfElements;

            if (maxNumberOfElements6 <= NUMBER_OF_NUMBERS_IN_LOTTERY6)
            {
                IsNext6Enabled = false;
            }
            else
            {
                IsNext6Enabled = true;
            }
        }
    }

    [RelayCommand]
    public async void NextPage5()
    {
        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumber>();

        PageableNumbers lottery5Numbers = await DatabaseService.GetLatestPageableNumbers(5, CurrentPage5 + 1);

        for (int i = 0; i < lottery5Numbers.GetAllNumbersList().Count; i++)
        {
            DisplayedLottery5Numbers.Add(new MyDrawableNumber(lottery5Numbers.GetAllNumbersList()[i], false));
        }

        CurrentPage5++;
        if (maxNumberOfElements5 <= CurrentPage5 * NUMBER_OF_NUMBERS_IN_LOTTERY5)
        {
            IsNext5Enabled = false;
        }
        IsPrev5Enabled = true;
    }

    [RelayCommand]
    public async void PrevPage5()
    {
        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumber>();

        PageableNumbers lottery5Numbers = await DatabaseService.GetLatestPageableNumbers(5, CurrentPage5 - 1);

        for (int i = 0; i < lottery5Numbers.GetAllNumbersList().Count; i++)
        {
            DisplayedLottery5Numbers.Add(new MyDrawableNumber(lottery5Numbers.GetAllNumbersList()[i], false));
        }

        CurrentPage5--;
        if (CurrentPage5 == 1)
        {
            IsPrev5Enabled = false;
        }
        IsNext5Enabled = true;
    }
}
