namespace BackpackBot.Services.Database
{
    using BackpackBot.Extensions;
    using BackpackBot.Services.Database.Models;
    using NLog;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DbService
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void SetupDb()
        {
            using (var conn = new SQLiteConnection(config.DbLocation))
            {
                try
                {
                    conn.CreateTable<DbPriceItem>();
                    conn.CreateTable<DbCurrency>();
                    conn.CreateTable<DbSpecialItem>();
                    conn.CreateTable<DbSchemaItem>();
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                    log.Error(ex, ex.StackTrace);
                }
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

        public static DbSchemaItem GetSchemaItem(uint defIndex)
        {
            DbSchemaItem item = null;
            using (var conn = new SQLiteConnection(config.DbLocation))
            {
                try
                {
                    item = conn.Get<DbSchemaItem>(defIndex);
                }
                catch (Exception ex)
                {
                    log.Warn(ex, ex.Message);
                    log.Warn(ex, ex.StackTrace);
                }
            }
            return item;
        }

        public static Dictionary<long, string> GetUniqueItems()
        {
            List<DbPriceItem> items = new List<DbPriceItem>();
            Dictionary<long, string> dict = new Dictionary<long, string>();
            using (var conn = new SQLiteConnection(config.DbLocation))
            {
                try
                {
                    items = conn.Query(conn.GetMapping<DbPriceItem>(), @"SELECT DISTINCT * FROM CommunityPrices").Cast<DbPriceItem>().ToList();
                    foreach (DbPriceItem i in items)
                    {
                        if (!dict.ContainsKey(i.DefIndex))
                        {
                            dict.Add(i.DefIndex, i.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Warn(ex, ex.Message);
                    log.Warn(ex, ex.StackTrace);
                }
            }
            return dict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
        
        public static bool TryGetDbPriceItem(string uniqueId, out DbPriceItem item)
        {
            using (var conn = new SQLiteConnection(config.DbLocation))
            {
                try
                {
                    item = conn.Get<DbPriceItem>(uniqueId);
                    return true;
                }
                catch (Exception ex)
                {
                    item = null;
                    log.Warn(ex, ex.Message);
                    log.Warn(ex, ex.StackTrace);
                }
            }
            return false;
        }
    }
}
