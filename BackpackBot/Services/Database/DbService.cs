namespace BackpackBot.Services.Database
{
    using BackpackBot.Services.Database.Models;
    using NLog;
    using SQLite;
    using System;
    using System.Collections.Generic;

    public class SQLiteConnection
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public SQLiteConnection(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
        
        public void Setup()
        {
            using (var db = new SQLiteConnection(Path))
            {
                try
                {
                    db.CreateTable<DbPriceItem>();
                    db.CreateTable<DbCurrency>();
                    db.CreateTable<DbSpecialItem>();
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                    log.Error(ex, ex.StackTrace);
                }
            }
        }

        
    }
}
