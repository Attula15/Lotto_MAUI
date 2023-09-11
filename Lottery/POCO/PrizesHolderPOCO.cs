using Lottery.Domain.Entity;

namespace Lottery.POCO;
public class PrizesHolderPOCO
{
    public List<PrizesEntity> prizes { get; set; }

    public PrizesHolderPOCO()
    {
        prizes = new List<PrizesEntity>();
    }
}

