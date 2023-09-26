using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.Mapper;
using Lottery.POCO;

namespace Lottery.Service;

public class CachingService
{
    private IRestAPI restApi;
    
    public CachingService(IRestAPI restApi)
    {
        this.restApi = restApi;
    }
    
    public async Task<MyNumbersPOCO> GetWinningNumbers(int type, string username)
    {
        WinningNumbersPOCO databaseWinningNumbers = WinningNumbersMapper.toPOCOFromDB(await DatabaseService.GetWinningNumbersFromCache(type, username));
        bool IsOld = DateTime.Now - databaseWinningNumbers.date > new TimeSpan(3, 0, 0, 0);
        bool isOverride;
        
        if (databaseWinningNumbers != null)
        {
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
        }
        

        if (isOverride || IsOld)
        {
            MyNumbersPOCO restApiWinnginNumbers = await restApi.GetWinningnumbers(type);
            await DatabaseService.AddWinningNumbersToCache(restApiWinnginNumbers.numbers, type, username);
            return restApiWinnginNumbers;
        }

        return MyNumberMapper.toPOCOFromWinning(databaseWinningNumbers);
    }
}