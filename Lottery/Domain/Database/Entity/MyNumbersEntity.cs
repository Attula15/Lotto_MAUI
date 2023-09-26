using SQLite;

namespace Lottery.Domain.Database.Entity;

[Table("MyNumbersTable")]
public class MyNumbersEntity
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public DateTime date { get; set; }

    [Indexed]
    public int numberType { get; set; }

    public string numbers { get; set; }

    public string username { get; set; }
}

