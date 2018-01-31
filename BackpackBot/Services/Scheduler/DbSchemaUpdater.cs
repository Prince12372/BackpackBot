using System;
using System.Collections.Generic;
using System.Text;

namespace BackpackBot.Services.Scheduler
{
    using FluentScheduler;
    using NLog;
    using SteamWebAPI2.Utilities;
    using SteamWebAPI2.Interfaces;
    using SteamWebAPI2.Models;

    public class DbSchemaUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static EconItems items = new EconItems(config.SteamApiKey, EconItemsAppId.TeamFortress2);

        public async void Execute()
        {
            var temp = await items.GetSchemaForTF2Async().ConfigureAwait(false);
        }
    }
}
