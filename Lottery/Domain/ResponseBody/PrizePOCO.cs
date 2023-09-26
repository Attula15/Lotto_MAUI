namespace Lottery.Domain.Entity;
public class PrizesPOCO
{
    public int prize { get; set; }
    public int whichOne { get; set; }
    public DateTime date { get; set; }

    public PrizesPOCO()
    {
    }

    public PrizesPOCO(int prize, int whichOne, DateTime date)
    {
        this.prize = prize;
        this.whichOne = whichOne;
        this.date = date;
    }
}
