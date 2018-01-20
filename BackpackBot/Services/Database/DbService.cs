namespace BackpackBot.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using BackpackBot.Services.Database.Models;
    using NLog;
    using SQLite;

    public class DbService
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public DbService(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
        
        public async Task Setup()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path);
            await db.CreateTablesAsync(types: new Type[] { typeof(DbCurrency), typeof(DbPriceItem), typeof(DbSpecialItem) }).ConfigureAwait(false);
        }
    }
}
