
namespace Lottery.Model;
public class MyDrawableNumber
{
    public int Number { get; set; }
    public bool IsDrawn { get; set; }

    public MyDrawableNumber(int number, bool isDrawn)
    {
        Number = number;
        IsDrawn = isDrawn;
    }
}