namespace BackpackBot.Modules.PriceCheck
{
    using CommandLine.Text;
    using CommandLine;
    using System.Linq;
    using BackpackBot.Extensions;
    using Discord;
    using Discord.Commands;
    using NLog;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using BackpackBot.Services.Database;
    using BackpackBot.Modules.Searches.Common;
    using BackpackBot.Services.Database.Models;

    public class PriceCheckCommands : ModuleBase<SocketCommandContext>
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        [Command("search")]
        [Alias("s")]
        [Summary("Searches for the top 5 closest matching items given a query.")]
        private async Task SearchAsync(params string[] input)
        {
            string query = string.Join(' ', input);
            var items = DbService.GetUniqueItems();
            List<string> closestMatch = StringExtensions.GetBestMatchesFor(items.Values.ToList(), query, 5);

            var eb = new EmbedBuilder()
                .WithSuccessColor()
                .AddField($"Closest matches for \"{query}\":", $"```\n{string.Join("\n", closestMatch)}\n```");

            await Context.Channel.SendMessageAsync(string.Empty, embed: eb.Build());
        }

        [Command("pricecheck")]
        [Alias("price", "pc")]
        [Summary("Gets pricing data for the closest match to a search string.")]
        private async Task PriceCheckAsync(params string[] input)
        {
            var eb = new EmbedBuilder();
            if (input == null || input.Length == 0)
            {
                eb.WithDescription("Please supply an item name to search for.")
                    .WithErrorColor();

                await Context.Channel.SendMessageAsync(string.Empty, embed: eb.Build()).ConfigureAwait(false);
                return;
            }

            var items = DbService.GetUniqueItems();

            var name = input.TakeWhile(x => !x.StartsWith('-')).ToList();
            var remainder = input.Skip(name.Count).ToList();

            // default values
            string quality = "6", priceIndex = "0";
            bool craftable = true, australium = false;

            long defindex = ItemExtensions.GetDefIndex(string.Join(' ', name), items);

            ItemSearchOptions options = new ItemSearchOptions();
            if (remainder.Count > 0)
            {
                var result = Parser.Default.ParseArguments<ItemSearchOptions>(remainder);
                options = result.MapResult(x => x, x => options);

                quality = ItemExtensions.GetQuality(string.Join(' ', options.Quality));
                priceIndex = ItemExtensions.GetPriceIndex(string.Join(' ', options.PriceIndex));
                craftable = options.Craftable;
                australium = options.Australium;
            }
            string uniqueId = ItemExtensions.CreateUniqueId(defindex, quality, craftable, priceIndex, australium);

            if (DbService.TryGetDbPriceItem(uniqueId, out DbPriceItem item))
            {
                // create the items's in-game name
                string itemName = ItemExtensions.GetQualityName(item.Quality) + " " + item.Name + (!priceIndex.Equals("0") ? $" ({ItemExtensions.GetEffectOrSeries(item.Name, item.PriceIndex)})" : string.Empty);

                // create the item's last update time
                string itemLastUpdate = item.LastUpdate.HasValue ? ItemExtensions.FormatLastUpdate(item.LastUpdate.Value) : "N/A";

                // create the item's displayed value
                string itemValue = item.Value.ToString() ?? "N/A";

                if (!itemValue.Equals("N/A"))
                {
                    if (item.Currency.Equals("metal"))
                        itemValue += " ref";
                    else if (item.Currency.Equals("keys"))
                        itemValue += " keys";
                    else if (item.Currency.Equals("usd"))
                        itemValue = "$" + itemValue;
                }

                eb.WithSuccessColor()
                   .WithTitle($"Price information for {itemName}")
                   .WithFooter($"Last site update for this item: {itemLastUpdate}");

                if (item.HighValue.HasValue)
                {
                    eb.AddInlineField("Lower Value:", itemValue);

                    string itemHighValue = item.HighValue.ToString();
                    if (item.Currency.Equals("metal"))
                        itemHighValue += " ref";
                    else if (item.Currency.Equals("keys"))
                        itemHighValue += " keys";
                    else if (item.Currency.Equals("usd"))
                        itemHighValue = "$" + itemHighValue;

                    eb.AddInlineField("Upper Value:", itemHighValue);
                }
                else
                {
                    eb.AddInlineField("Value:", itemValue);
                }

                if (item.Difference.HasValue)
                {
                    eb.AddInlineField("Difference:", item.Difference.Value.ToString("0.00") + " ref");
                }

                DbSchemaItem schemaItem = DbService.GetSchemaItem((uint)item.DefIndex);

                if (schemaItem != null && !string.IsNullOrWhiteSpace(schemaItem.ImageUrl))
                {
                    eb.WithThumbnailUrl(schemaItem.ImageUrl);
                }

                await Context.Channel.SendMessageAsync(string.Empty, embed: eb.Build()).ConfigureAwait(false);
            }
            else
            {
                eb.WithErrorColor()
                    .WithDescription("Couldn't find an item matching your search.")
                    .WithFooter($"Debug unique ID: {uniqueId}");
                await Context.Channel.SendMessageAsync(string.Empty, embed: eb.Build()).ConfigureAwait(false);
            }
        }
    }
}
