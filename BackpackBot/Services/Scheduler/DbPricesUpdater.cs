namespace BackpackBot.Services.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using BackpackBot.Extensions;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Database.Models;
    using BackpackWebAPI;
    using BackpackWebAPI.Models;
    using FluentScheduler;
    using NLog;

    public class DbPricesUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static BackpackWrapper wrapper = new BackpackWrapper(config.BackpackApiKey);

        public async void Execute()
        {
            log.Info("Update started");
            Stopwatch watch = Stopwatch.StartNew();
            CommunityPricesRoot root = null;
            try
            {
                root = await wrapper.GetCommunityPricesAsync().ConfigureAwait(false);
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
            List<DbPriceItem> items = new List<DbPriceItem>();

            foreach (var item in root.Response.Items)
            {
                foreach (long defindex in item.Value.DefIndexes)
                {
                    foreach (var quality in item.Value.Qualities)
                    {
                        Dictionary<string, IReadOnlyDictionary<string, ItemPrice>> craftabilities = new Dictionary<string, IReadOnlyDictionary<string, ItemPrice>>();
                        if (quality.Value.Tradable.Craftable != null)
                        {
                            craftabilities.Add("Craftable", quality.Value.Tradable.Craftable);
                        }
                        if (quality.Value.Tradable.NonCraftable != null)
                        {
                            craftabilities.Add("Non-Craftable", quality.Value.Tradable.NonCraftable);
                        }
                        foreach (var craftability in craftabilities)
                        {
                            foreach (var priceIndexKvp in craftability.Value)
                            {
                                items.Add(new DbPriceItem
                                    {
                                        UniqueId = ItemExtensions.CreateUniqueId(defindex, quality.Key, craftability.Key.Equals("Craftable"), priceIndexKvp.Key, priceIndexKvp.Value.Australium ?? false),
                                        DefIndex = defindex,
                                        Name = item.Key,
                                        Quality = quality.Key,
                                        Craftable = craftability.Key.Equals("Craftable"),
                                        PriceIndex = priceIndexKvp.Key,
                                        Australium = priceIndexKvp.Value.Australium,
                                        Currency = priceIndexKvp.Value.CurrencyType,
                                        Value = priceIndexKvp.Value.Value,
                                        HighValue = priceIndexKvp.Value.ValueHigh,
                                        LastUpdate = priceIndexKvp.Value.LastUpdate,
                                        Difference = priceIndexKvp.Value.Difference
                                });
                            }
                        }
                    }
                }
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
