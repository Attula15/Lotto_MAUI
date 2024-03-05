
namespace Lottery.Domain.ResponseBody;
public class SavedNumbersPOCO
{
    public string numbers { get; set; }
    public DateTime? date { get; set; }

    public SavedNumbersPOCO() 
    {
    }
    public SavedNumbersPOCO(string numbers, DateTime date)
    {
        this.numbers = numbers;
        this.date = date;
    }
}

