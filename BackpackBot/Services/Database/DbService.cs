namespace BackpackBot.Services.Database
{
    using BackpackBot.Services.Database.Models;
    using NLog;
    using SQLite;
    using System;
    using System.Collections.Generic;

    public class DbService
    {
        private Logger log = LogManager.GetCurrentClassLogger();
        public DbService(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }

        public void Setup()
        {
            using (var conn = new SQLiteConnection(Path))
            {
                try
                {
                    conn.CreateTable<DbPriceItem>();
                    conn.CreateTable<DbCurrency>();
                    conn.CreateTable<DbSpecialItem>();
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                    log.Error(ex, ex.StackTrace);
                }
            }
        }

        public (int, int) UpdateAndInsert<T>(List<T> items)
        {
            (int updated, int inserted) = (0, 0);
            using (var conn = new SQLiteConnection(Path))
            {
                try
                {
                    (updated, inserted) = (conn.UpdateAll(items), conn.InsertAll(items, "OR IGNORE"));
                }
                catch (Exception ex)
                {
                    log.Warn(ex, ex.Message);
                    log.Warn(ex, ex.StackTrace);
                }
            }

            return (updated, inserted);
        }
    }
}
