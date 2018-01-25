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

    public class DbSpecialItemsUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static BackpackWrapper wrapper = new BackpackWrapper(config.BackpackApiKey);

        public async void Execute()
        {
            log.Info("Update started");
            Stopwatch watch = Stopwatch.StartNew();
            SpecialItemsRoot root = null;
            try
            {
                root = await wrapper.GetSpecialItemsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Warn("Update did not complete successfully - see below");
                log.Warn(ex, ex.Message);
                if (ex.InnerException != null)
                {
                    log.Warn(ex.InnerException, ex.InnerException.Message);
                }
                return;
            }
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
                (int updated, int inserted) = DbService.UpdateAndInsert(items);
                log.Info($"Update completed - updated {updated} rows and inserted {inserted} new rows after {watch.ElapsedMilliseconds / 1000.0}s");
            }
            catch (Exception ex)
            {
                log.Warn("Update did not complete successfully - see below");
                log.Warn(ex, ex.Message);
            }
            watch.Stop();
        }
    }
}