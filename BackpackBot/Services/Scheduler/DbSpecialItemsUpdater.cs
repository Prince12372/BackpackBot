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

    public class DbSpecialItemsUpdater
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private Database.SQLiteConnection dbService;
        private BackpackWrapper wrapper;

        public DbSpecialItemsUpdater(Database.SQLiteConnection dbService, BackpackWrapper wrapper)
        {
            this.dbService = dbService;
            this.wrapper = wrapper;
        }

        public void Update()
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

            using (var db = new SQLite.SQLiteConnection(dbService.Path))
            {
                // First, insert new
                try
                {
                    inserted = db.InsertAll(items, extra: "OR IGNORE");
                }
                catch (Exception ex)
                {
                    log.Warn(ex, ex.Message, null);
                    log.Warn(ex, ex.StackTrace, null);
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
                        log.Warn(ex, ex.StackTrace, null);
                    }
                }
            }

            watch.Stop();
            log.Info($"Update complete - inserted {inserted} records and updated {updated} records after {watch.ElapsedMilliseconds / 1000.0}s");

        }
    }
}