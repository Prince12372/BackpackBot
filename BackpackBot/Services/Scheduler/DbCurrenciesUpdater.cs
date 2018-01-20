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

    public class DbCurrenciesUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static DbService db = new DbService();
        private static BackpackWrapper wrapper = new BackpackWrapper(config.BackpackApiKey);

        public void Execute()
        {
            log.Info("Update started.");
            CurrenciesRoot root = wrapper.GetCurrenciesAsync().Result;
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
                db.UpdateCurrencies(items);
                log.Info("Update complete.");
            }
            catch (Exception ex)
            {
                log.Warn(ex, ex.Message, null);
            }
        }
    }
}
