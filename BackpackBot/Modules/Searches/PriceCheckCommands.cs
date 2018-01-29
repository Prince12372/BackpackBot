namespace BackpackBot.Modules.PriceCheck
{
    using System.Linq;
    using BackpackBot.Extensions;
    using Discord;
    using Discord.Commands;
    using NLog;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using BackpackBot.Services.Database;

    public class PriceCheckCommands : ModuleBase<SocketCommandContext>
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        [Command("search")]
        [Alias("s")]
        [Summary("Searches for the top 5 closest matching items given a query.")]
        private async Task SearchAsync(params string[] input)
        {
            string query = string.Join(' ', input);
            List<string> items = DbService.GetUniqueItemNames();
            List<string> closestMatch = StringExtensions.GetBestMatchesFor(items, query, 5);

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
            if (input.Length == 0)
                return;

            var queryList = input.TakeWhile(x => !x.StartsWith('-')).ToList();
            var argList = input.TakeLast(input.Length - queryList.Count).ToList();

            string query = string.Join(' ', queryList);

            Dictionary<string, string> args = new Dictionary<string, string>();

            foreach (string argProp in argList)
            {
                if (argProp)
            }
        }
    }
}
