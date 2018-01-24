namespace BackpackBot.Services.Scheduler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Database.Models;
    using BackpackWebAPI;
    using BackpackWebAPI.Models;
    using NLog;

    public class DbSpecialItemsUpdater
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private DbService dbService;
        private BackpackWrapper wrapper;

        public DbSpecialItemsUpdater(DbService dbService, BackpackWrapper wrapper)
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

            dbService.UpdateAndInsert(items);
        }
    }
}