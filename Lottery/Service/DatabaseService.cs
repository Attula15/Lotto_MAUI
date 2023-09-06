
using Lottery.Domain.Database;
using Lottery.Domain.Database.Entity;
using SQLite;
using System.Diagnostics;

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

        var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData2.db3");

        try
        {
            db = new SQLiteAsyncConnection(databasePath); // Get an absolute path to the database file  
            await db.CreateTableAsync<MyNumbersEntity>();
            await db.DeleteAllAsync<MyNumbersEntity>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex + "\n\n");
        }
    }

    public static async Task<MyNumbersEntity> AddNumber(List<int> listOfNumbers, int type)
    {
        await Init();

        Debug.WriteLine("Saving started");
        DateTime currentDate = DateTime.Now;
        string numbersInList = "";

        //Convert list of numbers to storeable string
        for(int i = 0; i < listOfNumbers.Count; i++)
        {
            if(listOfNumbers.Count == i)
            {
                numbersInList = numbersInList + listOfNumbers[i] + "#";
            }
            else
            {
                numbersInList = numbersInList + listOfNumbers[i] + ";";
            }
        }
        Debug.WriteLine(numbersInList);

        try
        {
            MyNumbersEntity insertable = new MyNumbersEntity();
            numbersInList.Trim();
            insertable.numbers = numbersInList;
            insertable.date = currentDate;
            insertable.numberType = type;
            Debug.WriteLine("Database-ből: " + insertable.numbers.ToString());
            Debug.WriteLine("Database-ből: " + insertable.date);
            Debug.WriteLine("Database-ből: " + insertable.id);
            Debug.WriteLine("Database-ből: " + insertable.numberType);

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

    public static async Task<MyNumbersEntity> DeleteNumber(MyNumbersEntity number)
    {
        await Init();

        var q = db.Table<MyNumbersEntity>();
        q = q.Where(x => x.id.Equals(number.id));
        var myNumber = await q.FirstOrDefaultAsync();

        try
        {
            await db.DeleteAsync(number);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex + "\n\n");
        }

        return myNumber;
    }

    public static async Task DeleteAll()
    {
        await Init();

        await db.DeleteAllAsync<MyNumbersEntity>();
    }

    public static async Task<List<MyNumbersEntity>> GetAllNumbers()
    {
        await Init();
        var q = db.Table<MyNumbersEntity>();
        List<MyNumbersEntity> numbers = await q.ToListAsync();

        return numbers;
    }

    public static async Task<MyNumbersEntity> GetLatestNumbers(int type)
    {
        await Init();

        var q = db.Table<MyNumbersEntity>();
        return await q.Where(x => x.numberType == type).OrderByDescending(x => x.date).FirstOrDefaultAsync();
    }
    /*
    public static async Task<PageableNumbers> GetLatestPageableNumbers(int type, int page)
    {
        List<int> extractedNumbers = new List<int>();
        await Init();

        var q = db.Table<MyNumbersEntity>();
        var numbersFromDB = await q.Where(x => x.numberType == type).OrderByDescending(x => x.date).FirstOrDefaultAsync();

        numbersFromDB
    }*/
}

