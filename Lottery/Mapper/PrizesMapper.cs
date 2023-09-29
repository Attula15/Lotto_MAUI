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

    public static PrizesPOCO toPOCOFromPrizesForDataEntity(PrizesForDataEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new PrizesPOCO(entity.prize, entity.type, entity.date);
    }

    public static PrizesForDataEntity toDataEntityFromPOCO(PrizesPOCO poco, DateTime currentDate)
    {
        if (poco == null)
        {
            return null;
        }

        return new PrizesForDataEntity
        {
            prize = poco.prize,
            type = poco.whichOne,
            date = currentDate
        };
    }
}