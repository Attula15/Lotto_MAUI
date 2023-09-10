using Lottery.Domain.Entity;
using Lottery.POCO;

namespace Lottery.Domain;
public interface IRestAPI
{
    public Task<MyNumbersPOCO> GetWinningnumbers(String whichOne);
    public Task<PrizesHolderEntity> GetPrizes();
}

