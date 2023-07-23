using SQLite;

namespace Lottery.Domain.Database.Entity;
public class MyNumbers
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public DateTime date { get; set; }
    public string numbers { get; set; }
}

