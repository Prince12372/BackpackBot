namespace BackpackBot
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using BackpackBot.Services;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Scheduler;
    using BackpackWebAPI;
    using NLog;

    public class Program
    {
        public static void Main(string[] args)
            => MainAsync().GetAwaiter().GetResult();

        static async Task MainAsync()
        {
            Logging.SetupLogger();
            BotConfig config = new BotConfig();
            DbService dbService = new DbService(Path.Combine(Directory.GetCurrentDirectory(), "Data/BackpackBot.db"));
            BackpackWrapper wrapper = new BackpackWrapper(config.BackpackApiKey);
            DbSchedulerService scheduler = new DbSchedulerService(dbService, wrapper);

            scheduler.Start();

            await Task.Delay(-1);
        }
    }
}
