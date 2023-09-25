using SQLite;

namespace Lottery.Domain.Database.Entity;

[Table("TokenEntity")]
public class TokenEntity
{
    [PrimaryKey, AutoIncrement] public int id { get; set; }
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public string username { get; set; }
}