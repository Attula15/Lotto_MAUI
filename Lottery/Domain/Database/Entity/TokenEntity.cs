using SQLite;

namespace Lottery.Domain.Database.Entity;
[Table("Token")]
public class TokenEntity
{
    [PrimaryKey, AutoIncrement] 
    public int Id { get; set; }
    public string Token { get; set; }
}

