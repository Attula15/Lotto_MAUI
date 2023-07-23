
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
            await db.CreateTableAsync<MyNumbers>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex + "\n\n");
        }
    }

    public static async Task<MyNumbers> AddNumer(MyNumbers number)
    {
        await Init();

        try
        {
            await db.InsertAsync(number);
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex + "\n\n");
        }

        var q = db.Table<MyNumbers>();
        q = q.Where(x => x.id.Equals(number.id));
        var myNumber = await q.FirstOrDefaultAsync();

        return myNumber;
    }
}

