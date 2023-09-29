using SQLite;

namespace Lottery.Domain.Database.Entity;

[Table("PrizesForDataTable")]
public class PrizesForDataEntity
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    
    public int type { get; set; }
    
    public int prize { get; set; }
    
    public DateTime date { get; set; }
}