using Lottery.Domain.Entity;

namespace Lottery.Domain;
public interface IRestAPI
{
    public Task<WinningNumbersEntity> GetWinningnumbers(String whichOne);
    public Task<PrizesHolderEntity> GetPrizes();
}

