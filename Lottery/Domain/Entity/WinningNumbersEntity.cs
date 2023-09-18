
namespace Lottery.Domain.Entity;
public class WinningNumbersEntity
{
    public int id { get; set; }
    public DateTime date { get; set; }
    public string numbers { get; set; }
    public int whichOne { get; set; }
}
