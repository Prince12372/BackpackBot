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
        private string path;

        public DbService(string path)
        {
            this.path = path;
        }

        public async Task Setup()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(path);
            await db.CreateTablesAsync(types: new Type[] { typeof(DbCurrency), typeof(DbPriceItem), typeof(DbSpecialItem) }).ConfigureAwait(false);
        }
    }
}
