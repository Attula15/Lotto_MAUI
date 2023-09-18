
namespace Lottery.Domain.Database;
public class PageableNumbers
{
    public List<int> Numbers { get; set; }
    public int LotteryType { get; set; }
    public int MaxNumberOfElements { get; set; }

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
}
