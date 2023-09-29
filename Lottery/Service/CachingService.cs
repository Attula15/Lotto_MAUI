using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.Mapper;
using Lottery.POCO;

namespace Lottery.Service;

public class CachingService
{
    private IRestAPI restApi;

    private TimeSpan cachingTime = new TimeSpan(2, 0, 0, 0);
    
    public CachingService(IRestAPI restApi)
    {
        this.restApi = restApi;
    }
    
    public async Task<MyNumbersPOCO> GetWinningNumbers(int type)
    {
        WinningNumbersPOCO databaseWinningNumbers = WinningNumbersMapper.toPOCOFromDB(await DatabaseService.GetWinningNumbersFromCache(type));
        bool isOld;
        bool isOverride;
        
        if (databaseWinningNumbers != null)
        {
            isOld = DateTime.Now - databaseWinningNumbers.date > cachingTime;
            if (type == 5)
            {
                isOverride = DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday);
            }
            else
            {
                isOverride = DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday);
            }
        }
        else
        {
            isOverride = true;
            isOld = false;
        }
        

        if (isOverride || isOld)
        {
            MyNumbersPOCO restApiWinnginNumbers = await restApi.GetWinningnumbers(type);
            await DatabaseService.AddWinningNumbersToCache(restApiWinnginNumbers.numbers, type);
            return restApiWinnginNumbers;
        }

        return MyNumberMapper.toPOCOFromWinning(databaseWinningNumbers);
    }

    public async Task<PrizesHolderPOCO> GetLatestPrizes()
    {
        PrizesHolderPOCO prizesHolderPoco = await DatabaseService.GetLatestPrizesFromCache();

        if (prizesHolderPoco.prizes.Count > 0)
        {
            bool isOverride = DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday);

            if (isOverride)
            {
                prizesHolderPoco = await restApi.GetPrizes();
                foreach (PrizesPOCO prize in prizesHolderPoco.prizes)
                {
                    await DatabaseService.AddLatestPrizesToCache(prize.prize, prize.whichOne);
                }
            }
            else
            {
                PrizesPOCO prize5 = prizesHolderPoco.prizes.Find(x => x.whichOne == 5);
                PrizesPOCO prize6 = prizesHolderPoco.prizes.Find(x => x.whichOne == 6);
                
                bool isOld = DateTime.Now - prize5.date > cachingTime 
                             || DateTime.Now - prize6.date > cachingTime;

                if (isOld)
                {
                    prizesHolderPoco = await restApi.GetPrizes();
                    foreach (PrizesPOCO prize in prizesHolderPoco.prizes)
                    {
                        await DatabaseService.AddLatestPrizesToCache(prize.prize, prize.whichOne);
                    }
                }
            }
        }
        else
        {
            prizesHolderPoco = await restApi.GetPrizes();
            foreach (PrizesPOCO prize in prizesHolderPoco.prizes)
            {
                await DatabaseService.AddLatestPrizesToCache(prize.prize, prize.whichOne);
            }
        }

        return prizesHolderPoco;
    }

    public async Task<List<PrizesPOCO>> GetPrizes(int type)
    {
        List<PrizesPOCO> prizesFromDB = await DatabaseService.GetPrizesFromCache(type);
        List<PrizesPOCO> prizesFromAPI = null;

        if (prizesFromDB.Count > 0)
        {
            bool isOverride = DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday);

            if (isOverride)
            {
                prizesFromAPI = await restApi.getLastYearPrizes(type);
                await DatabaseService.AddPrizes(prizesFromAPI);
                
                return prizesFromAPI;
            }
            else
            {
                //Mi van, ha atlog az elozo honapba?
                var lastSunday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, (int)(DateTime.Now.Day - DateTime.Now.DayOfWeek));

                if (prizesFromDB[0].date < lastSunday)
                {
                    prizesFromAPI = await restApi.getLastYearPrizes(type);
                    await DatabaseService.AddPrizes(prizesFromAPI);
                    return prizesFromAPI;
                }

                return prizesFromDB;
            }
        }
        else
        {
            prizesFromAPI = await restApi.getLastYearPrizes(type);
            await DatabaseService.AddPrizes(prizesFromAPI);

            return prizesFromAPI;
        }
    }
}