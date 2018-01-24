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

    public class DbCurrenciesUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static BackpackWrapper wrapper = new BackpackWrapper(config.BackpackApiKey);

        public async void Execute()
        {
            log.Info("Update started.");
            Stopwatch watch = Stopwatch.StartNew();
            CurrenciesRoot root = null;
            try
            {
                root = await wrapper.GetCurrenciesAsync().ConfigureAwait(false);
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
            List<DbCurrency> items = new List<DbCurrency>();

            foreach (Currency c in root.Response.Currencies.Values)
            {
                items.Add(
                    new DbCurrency
                    {
                        Name = c.Name,
                        RoundTo = c.Round.GetValueOrDefault(),
                        Value = c.Price.Value,
                        HighValue = c.Price.HighValue,
                        ValueType = c.Price.CurrencyType,
                        Difference = c.Price.Difference,
                        LastUpdate = string.Format("{0:g}",
                                        new DateTime(1970, 1, 1, 0, 0, 0, 0)
                                            .AddSeconds(c.Price.LastUpdate))
                    });
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
                log.Warn(ex, ex.StackTrace);
            }
            watch.Stop();
        }
    }
}
