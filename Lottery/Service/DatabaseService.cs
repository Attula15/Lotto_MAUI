using Lottery.Domain.Database;
using Lottery.Domain.Database.Entity;
using Lottery.POCO;
using SQLite;
using System.Diagnostics;
using Lottery.Domain.Entity;
using Lottery.Mapper;

namespace Lottery.Service;
public static class DatabaseService
{
    private static SQLiteAsyncConnection db = null;

    private static async Task Init()
    {
        if (db != null)
        {
            return;
        }

        var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LotteryDatabaseV2.db3");

        try
        {
            db = new SQLiteAsyncConnection(databasePath); // Get an absolute path to the database file  
            await db.CreateTableAsync<MyNumbersEntity>();
            await db.CreateIndexAsync<MyNumbersEntity>(t => t.numberType);
            await db.CreateTableAsync<WinningNumbersDBEntity>();
            await db.CreateTableAsync<TokenEntity>();
            await db.CreateTableAsync<PrizesEntity>();
            //await db.DeleteAllAsync<MyNumbersEntity>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex + "\n\n");
        }
    }

    public static async Task<PrizesHolderPOCO> GetPrizesFromCache()
    {
        await Init();
        
        var q = db.Table<PrizesEntity>();
        var prizes = await q.ToListAsync();

        PrizesHolderPOCO returnable = new PrizesHolderPOCO();
        List<PrizesPOCO> prizesList = new List<PrizesPOCO>();
        
        foreach(PrizesEntity prize in prizes)
        {
            prizesList.Add(PrizesMapper.toPOCOFromPrizesDBEntity(prize));
        }

        returnable.prizes = prizesList;
        return returnable;
    }

    public static async Task AddPrizesToCache(int prize, int type)
    {
        await Init();
        
        PrizesEntity newEntity = new PrizesEntity();
        newEntity.date = DateTime.Now;
        newEntity.prize = prize;
        newEntity.whichOne = type;
        
        try
        {
            var q = db.Table<PrizesEntity>();
            var deletable = await q.Where(x => x.whichOne == type).FirstOrDefaultAsync();
            if (deletable != null)
            {
                await db.DeleteAsync<PrizesEntity>(deletable.id);
            }
            await db.InsertAsync(newEntity);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static async Task<WinningNumbersDBEntity> GetWinningNumbersFromCache(int type)
    {
        await Init();
        
        var q = db.Table<WinningNumbersDBEntity>();
        var returnable = await q.Where(x => x.numberType == type).FirstOrDefaultAsync();

        return returnable;
    }

    
    public static async Task AddWinningNumbersToCache(List<int> numbers, int type)
    {
        await Init();

        string numbersInString = "";
        
        //Convert list of numbers to storeable string
        for(int i = 0; i < numbers.Count; i++)
        {
            numbersInString = numbersInString + numbers[i] + ";";
        }
        
        WinningNumbersDBEntity newEntity = new WinningNumbersDBEntity();
        newEntity.date = DateTime.Now;
        newEntity.numberType = type;
        newEntity.numbers = numbersInString;

        try
        {
            var q = db.Table<WinningNumbersDBEntity>();
            var deletable = await q.Where(x => x.numberType == type).FirstOrDefaultAsync();
            if (deletable != null)
            {
                await db.DeleteAsync<WinningNumbersDBEntity>(deletable.id);
            }
            await db.InsertAsync(newEntity);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static async Task AddTokens(string accessToken, string refreshToken, string username)
    {
        await Init();
        
        TokenEntity newEntity = new TokenEntity
        {
            access_token = accessToken,
            refresh_token = refreshToken,
            username = username
        };

        try
        {
            await db.DeleteAllAsync<TokenEntity>();
            await db.InsertAsync(newEntity);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error while adding new access Token");
            Debug.WriteLine(ex.Message);
        }
    }

    public static async Task<TokenEntity> GetToken(string username)
    {
        await Init();

        var q = db.Table<TokenEntity>();
        return await q.Where(x => x.username.Equals(username)).FirstOrDefaultAsync();
    }

    public static async Task<MyNumbersEntity> AddNumber(List<int> listOfNumbers, int type, string username)
    {
        await Init();

        DateTime currentDate = DateTime.Now;
        string numbersInList = "";

        //Convert list of numbers to storeable string
        for(int i = 0; i < listOfNumbers.Count; i++)
        {
            numbersInList = numbersInList + listOfNumbers[i] + ";";
        }

        try
        {
            MyNumbersEntity insertable = new MyNumbersEntity();
            numbersInList.Trim();
            insertable.numbers = numbersInList;
            insertable.date = currentDate;
            insertable.numberType = type;
            insertable.username = username;

            await db.InsertAsync(insertable);
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex + "\n\n");
        }

        var q = db.Table<MyNumbersEntity>();
        var myNumber = await q.OrderByDescending(x => x.date).FirstOrDefaultAsync();

        return myNumber;
    }

    public static async Task<MyNumbersPOCO> GetLatestNumbers(int type, string username)
    {
        await Init();

        var q = db.Table<MyNumbersEntity>();
        var dataFromDB = await q.Where(x => x.numberType == type && x.username.Equals(username)).OrderByDescending(x => x.date).FirstOrDefaultAsync();

        if(dataFromDB == null)
        {
            return null;
        }

        return MyNumberMapper.toPOCOFromDBEntity(dataFromDB);
    }
}

