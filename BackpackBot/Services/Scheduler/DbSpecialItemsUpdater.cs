namespace BackpackBot.Services.Scheduler
{
    using System;
    using System.Collections.Generic;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Database.Models;
    using BackpackWebAPI;
    using BackpackWebAPI.Models;
    using FluentScheduler;
    using NLog;

    public class DbSpecialItemsUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static DbService db = new DbService();
        private static BackpackWrapper wrapper = new BackpackWrapper(config.BackpackApiKey);

        public void Execute()
        {
            log.Info("Update started.");
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
            try
            {
                db.UpdateSpecialItems(items);
                log.Info("Update complete.");
            }
            catch (Exception ex)
            {
                log.Warn(ex, ex.Message, null);
            }
        }
    }
}