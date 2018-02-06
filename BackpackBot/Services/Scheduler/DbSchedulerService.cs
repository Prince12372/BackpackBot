namespace BackpackBot.Services.Scheduler
{
    using System.Linq;
    using FluentScheduler;
    using NLog;
    using System.Collections.Generic;
    using System;

    public static class DbSchedulerService
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static Registry registry = new Registry();

        public static void Initialize()
        {
            log.Info("Scheduler service started.");

            //registry.NonReentrantAsDefault();

            registry.Schedule<DbPricesUpdater>().WithName("prices").ToRunNow().AndEvery(30).Minutes();
            registry.Schedule<DbCurrenciesUpdater>().WithName("currencies").ToRunNow().AndEvery(1).Hours();
            registry.Schedule<DbSpecialItemsUpdater>().WithName("specials").ToRunNow().AndEvery(24).Hours();
            registry.Schedule<DbSchemaUpdater>().WithName("schema").ToRunNow().AndEvery(24).Hours();

            JobManager.Initialize(registry);

            JobManager.JobException += jobException => log.Fatal($"An error just happened with a scheduled job: {jobException.Exception}");
        }
    }
}
