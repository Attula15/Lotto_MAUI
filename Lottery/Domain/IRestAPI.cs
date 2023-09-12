using Lottery.POCO;

namespace Lottery.Domain;
public interface IRestAPI
{
    public Task<MyNumbersPOCO> GetWinningnumbers(String whichOne);
    public Task<PrizesHolderPOCO> GetPrizes();
    public Task<bool> uploadNumbers(List<int> numbers, int whichOne);
    public Task<bool> logOut();
}

