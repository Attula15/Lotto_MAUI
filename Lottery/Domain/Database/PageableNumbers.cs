
namespace Lottery.Domain.Database;
public class PageableNumbers
{
    private List<int> Numbers;
    private int LotteryType;
    private int MaxPages { get; set; }
    private int MaxNumberOfElements { get; set; }

    public PageableNumbers(int lotteryType)
    {
        Numbers = new List<int>();
        LotteryType = lotteryType;
    }

    public List<int> GetAllNumbersList()
    {
        return Numbers;
    }

    public void AppendNumber(int number)
    {
        Numbers.Add(number);
    }

    public int GetType()
    {
        return LotteryType;
    }
}
