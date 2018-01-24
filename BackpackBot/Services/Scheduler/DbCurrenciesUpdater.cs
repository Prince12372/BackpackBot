namespace BackpackBot.Services.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Database.Models;
    using BackpackWebAPI;
    using BackpackWebAPI.Models;
    using NLog;

    public class DbCurrenciesUpdater
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private DbService dbService;
        private BackpackWrapper wrapper;

        public DbCurrenciesUpdater(DbService dbService, BackpackWrapper wrapper)
        {
            this.dbService = dbService;
            this.wrapper = wrapper;
        }

        public void Update()
        {
            log.Info("Update started.");
            Stopwatch watch = Stopwatch.StartNew();
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

            dbService.UpdateAndInsert(items);
        }
    }
}
