using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Lottery.Domain.Database.Entity;
public class MyNumbersEntity
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public DateTime date { get; set; }
    
    public int numberType { get; set; }

    public string numbers { get; set; }
}

