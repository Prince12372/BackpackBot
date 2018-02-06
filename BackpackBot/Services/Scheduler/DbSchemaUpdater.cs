namespace BackpackBot.Services.Scheduler
{
    using System;
    using System.Collections.Generic;
    using FluentScheduler;
    using NLog;
    using SteamWebAPI2.Utilities;
    using SteamWebAPI2.Interfaces;
    using System.Diagnostics;
    using BackpackBot.Services.Database.Models;
    using BackpackBot.Services.Database;

    public class DbSchemaUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static EconItems econ = new EconItems(config.SteamApiKey, EconItemsAppId.TeamFortress2);

        public async void Execute()
        {
            log.Info("Update started");
            Stopwatch watch = Stopwatch.StartNew();
            ISteamWebResponse<Steam.Models.TF2.SchemaModel> schema = null;

            try
            {
                schema = await econ.GetSchemaForTF2Async().ConfigureAwait(false);
                List<DbSchemaItem> items = new List<DbSchemaItem>();

                foreach (var item in schema.Data.Items)
                {
                    items.Add(new DbSchemaItem
                    {
                        DefIndex = item.DefIndex,
                        Name = item.ItemName,
                        ImageUrl = item.ImageUrl,
                        ImageUrlLarge = item.ImageUrlLarge
                    });
                }

                (int updated, int inserted) = DbService.UpdateAndInsert(items);
                log.Info($"Update completed - updated {updated} rows and inserted {inserted} new rows after {watch.ElapsedMilliseconds / 1000.0}s");
            }
            catch (Exception ex)
            {
                log.Warn("Update did not complete successfully - see below");
                log.Warn(ex, ex.Message);
                if (ex.InnerException != null)
                {
                    log.Warn(ex.InnerException, ex.InnerException.Message);
                }
            }
            watch.Stop();
        }
    }
}
