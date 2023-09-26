using Lottery.Domain.Database.Entity;
using Lottery.Domain.Entity;

namespace Lottery.Mapper;

public class PrizesMapper
{
    public static PrizesPOCO toPOCOFromPrizesDBEntity(PrizesEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new PrizesPOCO(entity.prize, entity.whichOne, entity.date);
    }
}