using CommunityToolkit.Mvvm.ComponentModel;
using Lottery.POCO;
using Lottery.Service;
using System.Collections.ObjectModel;

namespace Lottery.ViewModel;
public partial class SavedPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumberPOCO> displayedLottery5Numbers = new ObservableCollection<MyDrawableNumberPOCO>();
    [ObservableProperty]
    private ObservableCollection<MyDrawableNumberPOCO> displayedLottery6Numbers = new ObservableCollection<MyDrawableNumberPOCO>();

    [ObservableProperty]
    private bool isLoading = false;

    public async void LoadNumbersIntoDrawnList()
    {
        IsLoading = true;

        DisplayedLottery5Numbers = new ObservableCollection<MyDrawableNumberPOCO>();
        DisplayedLottery6Numbers = new ObservableCollection<MyDrawableNumberPOCO>();

        MyNumbersPOCO lottery5Numbers = await DatabaseService.GetLatestNumbers(5);
        MyNumbersPOCO lottery6Numbers = await DatabaseService.GetLatestNumbers(6);

        ObservableCollection<MyDrawableNumberPOCO> temp5 = new ObservableCollection<MyDrawableNumberPOCO>();
        ObservableCollection<MyDrawableNumberPOCO> temp6 = new ObservableCollection<MyDrawableNumberPOCO>();

        if (lottery5Numbers != null)
        {
            for (int i = 0; i < lottery5Numbers.numbers.Count; i++)
            {
                temp5.Add(new MyDrawableNumberPOCO(lottery5Numbers.numbers[i], false));
            }
        }

        DisplayedLottery5Numbers = temp5;

        if (lottery6Numbers != null)
        {
            for (int i = 0; i < lottery6Numbers.numbers.Count; i++)
            {
                temp6.Add(new MyDrawableNumberPOCO(lottery6Numbers.numbers[i], false));
            }
        }

        DisplayedLottery6Numbers = temp6;

        IsLoading = false;
    }
}
