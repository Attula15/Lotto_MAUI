using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Lottery.Model;
public partial class MyDrawableNumbers : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<MyDrawableNumber> drawnNumbers = new ObservableCollection<MyDrawableNumber>();

    public void Append(MyDrawableNumber listOfNumbers)
    {
        DrawnNumbers.Add(listOfNumbers);
    }

    public bool Contains(MyDrawableNumber listOfNumbers)
    {
        return DrawnNumbers.Contains(listOfNumbers);
    }

    public List<int> GetNumbers()
    {
        List<int> returnable = new List<int>();
        for(int i = 0; i <  DrawnNumbers.Count; i++)
        {
            returnable.Add(DrawnNumbers[i].Number);
        }
        return returnable;
    }
}
