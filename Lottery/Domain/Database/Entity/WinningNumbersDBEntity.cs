using SQLite;

namespace Lottery.Domain.Database.Entity;

[Table("WinningNumbersEntity")]
public class WinningNumbersDBEntity
{
    [PrimaryKey, AutoIncrement] 
    public int id { get; set; }
    
    public DateTime date { get; set; }

    [Indexed]
    public int numberType { get; set; }

    public string numbers { get; set; }
}