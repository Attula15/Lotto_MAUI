
namespace Lottery.Domain.Entity;
public class WinningNumbersPOCO
{
    public int id { get; set; }
    public DateTime date { get; set; }
    public string numbers { get; set; }
    public int whichOne { get; set; }

    public WinningNumbersPOCO()
    {
    }

    public WinningNumbersPOCO(int id, DateTime date, string numbers, int whichOne)
    {
        this.id = id;
        this.date = date;
        this.numbers = numbers;
        this.whichOne = whichOne;
    }
}
