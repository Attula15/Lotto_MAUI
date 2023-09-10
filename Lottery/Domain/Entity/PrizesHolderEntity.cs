
namespace Lottery.Domain.Entity;
public class PrizesHolderEntity
{
    public List<PrizesEntity> prizes {  get; set; }

    public PrizesHolderEntity()
    {
        this.prizes = new List<PrizesEntity>();
    }
}

