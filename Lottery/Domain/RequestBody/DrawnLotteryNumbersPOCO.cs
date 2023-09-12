namespace Lottery.Domain.RequestBody;
public class DrawnLotteryNumbersPOCO
{
    public string numbers { get; set; }
    public int whichOne { get; set; }

    public DrawnLotteryNumbersPOCO(string numbers, int whichOne) 
    { 
        this.numbers = numbers;
        this.whichOne = whichOne;
    }
}

