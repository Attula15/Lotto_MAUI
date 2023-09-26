using Lottery.Domain.Database.Entity;
using Lottery.Domain.Entity;

namespace Lottery.Mapper;

public class WinningNumbersMapper
{
    public static WinningNumbersPOCO toPOCOFromDB(WinningNumbersDBEntity entity)
    {
        if (entity == null)
        {
            return null;
        }
        return new WinningNumbersPOCO(entity.id, entity.date, entity.numbers, entity.numberType);
    }
}