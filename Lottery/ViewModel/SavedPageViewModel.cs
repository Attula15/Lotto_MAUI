using CommunityToolkit.Mvvm.ComponentModel;
using Lottery.Model;
using Lottery.POCO;
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
    private bool isLoading = false;

    public async void LoadNumbersIntoDrawnList()
    {
        IsLoading = true;

        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumber>();
        DisplayedLottery6Numbers = new ObservableCollection<MyDrawableNumber>();

        MyNumbersPOCO lottery5Numbers = await DatabaseService.GetLatestNumbers(5);
        MyNumbersPOCO lottery6Numbers = await DatabaseService.GetLatestNumbers(6);

        ObservableCollection<MyDrawableNumber> temp5 = new ObservableCollection<MyDrawableNumber>();
        ObservableCollection<MyDrawableNumber> temp6 = new ObservableCollection<MyDrawableNumber>();

        if (lottery5Numbers != null)
        {
            for (int i = 0; i < lottery5Numbers.numbers.Count; i++)
            {
                temp5.Add(new MyDrawableNumber(lottery5Numbers.numbers[i], false));
            }
        }

        DisplayedLottery5Numbers = temp5;

        if (lottery6Numbers != null)
        {
            for (int i = 0; i < lottery6Numbers.numbers.Count; i++)
            {
                temp6.Add(new MyDrawableNumber(lottery6Numbers.numbers[i], false));
            }
        }

        DisplayedLottery6Numbers = temp6;

        IsLoading = false;
    }
}
