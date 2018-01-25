namespace BackpackBot.Services.Scheduler
{
    using FluentScheduler;
    using NLog;

    public static class DbSchedulerService
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static Registry registry = new Registry();

        public static void Start()
        {
            log.Info("Scheduler service started.");
            registry.Schedule<DbPricesUpdater>().NonReentrant().ToRunNow().AndEvery(1).Hours().DelayFor(5).Minutes();
            registry.Schedule<DbCurrenciesUpdater>().NonReentrant().ToRunNow().AndEvery(1).Hours();
            registry.Schedule<DbSpecialItemsUpdater>().NonReentrant().ToRunNow().AndEvery(1).Weeks();
            JobManager.Initialize(registry);
            JobManager.JobException += info => log.Fatal("An error just happened with a scheduled job: " + info.Exception);
        }

        public static void Stop()
        {
            log.Info("Scheduler service stopped.");
            JobManager.Stop();
        }
    }
}
