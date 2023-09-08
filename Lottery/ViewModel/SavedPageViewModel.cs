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

    public async void LoadNumbersIntoDrawnList()
    {
        MyNumbersPOCO lottery5Numbers = await DatabaseService.GetLatestNumbers(5);
        MyNumbersPOCO lottery6Numbers = await DatabaseService.GetLatestNumbers(6);

        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumber>();
        DisplayedLottery6Numbers = new ObservableCollection<MyDrawableNumber>();

        if (lottery5Numbers != null)
        {
            for (int i = 0; i < lottery5Numbers.numbers.Count; i++)
            {
                DisplayedLottery5Numbers.Add(new MyDrawableNumber(lottery5Numbers.numbers[i], false));
            }
        }

        if (lottery6Numbers != null)
        {
            for (int i = 0; i < lottery6Numbers.numbers.Count; i++)
            {
                DisplayedLottery6Numbers.Add(new MyDrawableNumber(lottery6Numbers.numbers[i], false));
            }
        }
    }
}
