
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

        var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db3");

        try
        {
            db = new SQLiteAsyncConnection(databasePath); // Get an absolute path to the database file  
            await db.CreateTableAsync<MyNumbersEntity>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex + "\n\n");
        }
    }

    public static async Task<MyNumbersEntity> AddNumer(string number, int type)
    {
        await Init();

        try
        {
            MyNumbersEntity insertable = new MyNumbersEntity();
            insertable.numbers = number;
            insertable.date = DateTime.Now;
            insertable.numberType = type;
            Debug.WriteLine("Database-ből: " + insertable.numbers);
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
        q = q.Where(x => x.numbers.Equals(number));
        var myNumber = await q.FirstOrDefaultAsync();

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

    public static async Task<PageableNumbers> GetLatestPageableNumbers(int type, int page)
    {

    }
}

