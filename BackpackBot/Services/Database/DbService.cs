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
        
        public void Setup()
        {
            using (var db = new SQLiteConnection(Path))
            {
                db.CreateTable<DbCurrency>();
                db.CreateTable<DbPriceItem>();
                db.CreateTable<DbSpecialItem>();
            }
        }
    }
}
