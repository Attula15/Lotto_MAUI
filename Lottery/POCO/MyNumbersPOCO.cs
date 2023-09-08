
namespace Lottery.POCO;

public class MyNumbersPOCO
{
    public int id { get; set; }
    public DateTime date { get; set; }

    public int numberType { get; set; }

    public List<int> numbers { get; set; }

    public MyNumbersPOCO(int id, DateTime date, int numberType, List<int> numbers) 
    {
        this.id = id;
        this.date = date;
        this.numberType = numberType;
        this.numbers = numbers;
    }
}

