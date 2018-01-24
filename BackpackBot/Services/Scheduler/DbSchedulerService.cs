namespace BackpackBot.Services.Scheduler
{
    using BackpackBot.Services.Database;
    using BackpackWebAPI;
    using FluentScheduler;
    using NLog;

    public class DbSchedulerService : Registry
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();

        public DbSchedulerService(DbService db, BackpackWrapper wrapper)
        {
            DbPricesUpdater pricesUpdater = new DbPricesUpdater(db, wrapper);
            DbCurrenciesUpdater currenciesUpdater = new DbCurrenciesUpdater(db, wrapper);
            DbSpecialItemsUpdater specialItemsUpdater = new DbSpecialItemsUpdater(db, wrapper);

            Schedule(() => pricesUpdater.Update()).NonReentrant().ToRunNow().AndEvery(30).Minutes();
            Schedule(() => currenciesUpdater.Update()).NonReentrant().ToRunNow().AndEvery(1).Hours();
            Schedule(() => specialItemsUpdater.Update()).NonReentrant().ToRunNow().AndEvery(1).Weeks(); ;
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
