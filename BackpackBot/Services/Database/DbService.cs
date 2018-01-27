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

        public static List<string> GetUniqueItemNames()
        {
            using (var conn = new SQLiteConnection(config.DbLocation))
            {
                try
                {
                    var items = conn.Query(conn.GetMapping<DbPriceItem>(), @"SELECT Name FROM CommunityPrices").Cast<DbPriceItem>().ToList();
                    return items.Select(c => c.Name).Distinct().ToList();
                }
                catch (Exception ex)
                {
                    log.Warn(ex, ex.Message);
                    log.Warn(ex, ex.StackTrace);
                }
            }

            return new List<string>()
            {
                "No unique items found."
            };
        }
        
        public static bool TryGetDbPriceItem(long defindex, string quality, bool craftable, string priceIndex, bool australium, out DbPriceItem item)
        {
            string uniqueId = ItemExtensions.CreateUniqueId(defindex, quality, craftable, priceIndex, australium);
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
