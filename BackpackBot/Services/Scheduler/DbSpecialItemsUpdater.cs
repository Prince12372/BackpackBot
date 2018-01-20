namespace BackpackBot.Services.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Database.Models;
    using BackpackWebAPI;
    using BackpackWebAPI.Models;
    using FluentScheduler;
    using NLog;
    using SQLite;

    public class DbSpecialItemsUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private DbService dbService;
        private BackpackWrapper wrapper;

        public DbSpecialItemsUpdater()
        {
        }

        public void Execute()
        {
            log.Info("Update started.");
            Stopwatch watch = Stopwatch.StartNew();
            SpecialItemsRoot root = wrapper.GetSpecialItemsAsync().Result;
            List<DbSpecialItem> items = new List<DbSpecialItem>();

            foreach (SpecialItem i in root.Response.Items)
            {
                items.Add(
                    new DbSpecialItem
                    {
                        InternalName = i.InternalName,
                        ItemName = i.ItemName,
                        DefIndex = i.DefIndex.GetValueOrDefault(),
                        Description = i.Description,
                        AppId = i.AppId.GetValueOrDefault()
                    }
                );
            }

            int inserted = 0, updated = 0;

            using (var db = new SQLiteConnection(dbService.Path))
            {
                // First, insert new
                try
                {
                    inserted = db.InsertAll(items, "IF NOT EXISTS");
                }
                catch (Exception ex)
                {
                    log.Warn(ex, ex.Message, null);
                }

                // Second, update
                {
                    try
                    {
                        updated = db.UpdateAll(items);
                    }
                    catch (Exception ex)
                    {
                        log.Warn(ex, ex.Message, null);
                    }
                }
            }

            watch.Stop();
            log.Info($"Update complete - Inserted {inserted} items and updated {updated} items - took {watch.ElapsedMilliseconds}ms");

        }

        public void Setup(DbService dbService, BackpackWrapper wrapper)
        {
            this.dbService = dbService;
            this.wrapper = wrapper;
        }
    }
}