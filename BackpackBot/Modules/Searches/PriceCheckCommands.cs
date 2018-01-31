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
            string quality = "1", priceIndex = "0";
            bool craftable = true, australium = false;

            long defindex = ItemExtensions.GetDefIndex(string.Join(' ', name), items);

            ItemSearchOptions options = new ItemSearchOptions();
            if (!name.Equals(remainder))
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
                eb.WithTitle($"Price information for {item.Name} {(!quality.Equals("0") ? $"with effect {ItemExtensions.GetEffectOrSeries(item.Name, priceIndex)}" : string.Empty)}")
                    .WithFooter($"Last updated: {(item.LastUpdate.HasValue ? item.LastUpdate.Value.ToString("g") : default)}")
                    .AddInlineField("Current Value", $"{item.Value ?? default} {item.Currency ?? string.Empty}")
                    .AddInlineField("High Value", item.HighValue ?? default)
                    .AddInlineField("Difference", item.Difference ?? default)
                    .WithSuccessColor();

                await Context.Channel.SendMessageAsync(string.Empty, embed: eb.Build()).ConfigureAwait(false);
            }
        }
    }
}
