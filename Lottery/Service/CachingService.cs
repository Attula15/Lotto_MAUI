using System.Diagnostics;
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

    public async Task<PrizesHolderPOCO> GetPrizes()
    {
        bool canGetFromAPI = true;
        PrizesHolderPOCO prizesHolderPoco = await DatabaseService.GetPrizesFromCache();

        if (prizesHolderPoco != null && prizesHolderPoco.prizes.Count > 0)
        {
            bool isOverride = DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday);

            if (isOverride)
            {
                prizesHolderPoco = await restApi.GetPrizes();
                if (prizesHolderPoco != null)
                {
                    foreach (PrizesPOCO prize in prizesHolderPoco.prizes)
                    {
                        await DatabaseService.AddPrizesToCache(prize.prize, prize.whichOne);
                    }
                }
                else
                {
                    canGetFromAPI = false;
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
                    if (prizesHolderPoco != null)
                    {
                        foreach (PrizesPOCO prize in prizesHolderPoco.prizes)
                        {
                            await DatabaseService.AddPrizesToCache(prize.prize, prize.whichOne);
                        }
                    }
                    else
                    {
                        canGetFromAPI = false;
                    }
                }
            }
        }
        else
        {
            prizesHolderPoco = await restApi.GetPrizes();
            if (prizesHolderPoco != null)
            {
                foreach (PrizesPOCO prize in prizesHolderPoco.prizes)
                {
                    await DatabaseService.AddPrizesToCache(prize.prize, prize.whichOne);
                }
            }
            else
            {
                canGetFromAPI = false;
            }
        }

        if (!canGetFromAPI)
        {
            Debug.WriteLine("Couldn't access the API");
            return null;
        }

        return prizesHolderPoco;
    }
}