namespace BackpackBot
{
    using System;
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

            DbSchedulerService scheduler = new DbSchedulerService();

            scheduler.Start();

            await Task.Delay(-1);
        }
    }
}
