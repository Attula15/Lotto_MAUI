using Lottery.Domain.Entity;
using Lottery.Domain.ResponseBody;
using Lottery.POCO;

namespace Lottery.Domain;
public interface IRestAPI
{
    public Task<MyNumbersPOCO> GetWinningnumbers(int whichOne);
    public Task<PrizesHolderPOCO> GetPrizes();
    public Task<bool> uploadNumbers(List<int> numbers, int whichOne);
    public Task<SavedNumbersPOCO> getSavedNumbersFromAPI(int whichOne);
    public Task<List<PrizesPOCO>> getLastYearPrizes(string whichOne);
    public Task<List<LotteryWinnersDataPOCO>> getLatestWinnersData(int whichOne);
}

