
namespace Lottery.POCO;
public class MyDrawableNumberPOCO
{
    public int Number { get; set; }
    public bool IsDrawn { get; set; }
    public bool IsDrawnNot { get; set; }

    public MyDrawableNumberPOCO(int number, bool isDrawn)
    {
        Number = number;
        IsDrawn = isDrawn;
        IsDrawnNot = !isDrawn;
    }
}