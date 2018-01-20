namespace BackpackBot.Services.Scheduler
{
    using BackpackBot.Services.Database;
    using FluentScheduler;
    using NLog;

    public class DbSchedulerService : Registry
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();

        public DbSchedulerService()
        {
            DbService db = new DbService();
            db.Setup();
            Schedule<DbCurrenciesUpdater>().ToRunNow().AndEvery(1).Hours();
            Schedule<DbPricesUpdater>().ToRunNow().AndEvery(1).Hours();
            Schedule<DbSpecialItemsUpdater>().ToRunNow();
        }

        public void Start()
        {
            log.Info("Scheduler service started.");
            JobManager.Initialize(this);
            JobManager.JobException += info => log.Fatal("An error just happened with a scheduled job: " + info.Exception);
        }

        public void Stop()
        {
            log.Info("Scheduler service stopped.");
            JobManager.Stop();
        }
    }
}
