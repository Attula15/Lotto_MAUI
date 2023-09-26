using SQLite;

namespace Lottery.Domain.Database.Entity;

[Table("PrizesTable")]
public class PrizesEntity
{
    [PrimaryKey, AutoIncrement] public int id { get; set; }

    public int whichOne { get; set; }

    public DateTime date { get; set; }

    public int prize { get; set; }
}