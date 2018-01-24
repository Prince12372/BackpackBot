namespace BackpackBot.Services.Database
{
    using BackpackBot.Services.Database.Models;
    using NLog;
    using SQLite;
    using System;
    using System.Collections.Generic;

    public static class DbService
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static (int, int, int) SetupDb()
        {
            using (var conn = new SQLiteConnection(config.DbLocation))
            {
                try
                {
                    return (conn.CreateTable<DbPriceItem>(), conn.CreateTable<DbCurrency>(), conn.CreateTable<DbSpecialItem>());
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                    log.Error(ex, ex.StackTrace);
                }
                return (default, default, default);
            }
        }

        public static (int, int) UpdateAndInsert<T>(List<T> items)
        {
            using (var conn = new SQLiteConnection(config.DbLocation))
            {
                try
                {
                    return (conn.UpdateAll(items), conn.InsertAll(items, "OR IGNORE"));
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                    log.Error(ex, ex.StackTrace);
                }
                return (default, default);
            }
        } 
    }
}
