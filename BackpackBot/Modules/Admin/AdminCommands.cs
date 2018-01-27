namespace BackpackBot.Modules.Admin
{
    using System.Linq;
    using System.Threading.Tasks;
    using BackpackBot.Extensions;
    using BackpackBot.Services;
    using BackpackBot.Services.Scheduler;
    using Discord;
    using Discord.Commands;
    using FluentScheduler;
    using NLog;

    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        private Logger log = LogManager.GetCurrentClassLogger();
        private BotConfig config = new BotConfig();

        [Command("scheduler")]
        [Summary("Bot owner only. Used to enable/disable the database scheduler or check its status.")]
        private async Task SchedulerAsync(params string[] input)
        {
            if (config.OwnerIds.Contains(Context.Message.Author.Id))
            {
                /*
                 * \U00002705 -> :white_check_mark:
                 * \U0001f44c -> :ok_hand:
                 * \U0001f6d1 -> :octagonal_sign:
                 * \U0001f501 -> :repeat:
                 * \U00002753 -> :question:
                 * \U000026a0 -> :warning:
                 * \U000023e9 -> :fast_forward:
                 * await Context.Message.AddReactionAsync(new Emoji("\U00002705")).ConfigureAwait(false);
                 */

                if (input.Length < 2)
                    return;

                string type = input[0].ToLower(), action = input[1].ToLower();

                if (!(type.Equals("prices") || type.Equals("currencies") || type.Equals("specials")) || !(action.Equals("start") || action.Equals("stop") || action.Equals("status") || action.Equals("next")))
                {
                    await Context.Message.AddReactionAsync(new Emoji("\U00002753")).ConfigureAwait(false);
                    return;
                }

                switch (action)
                {
                    case "start":
                        if (JobManager.AllSchedules.First(y => y.Name.Equals(type)).Disabled)
                        {
                            JobManager.AllSchedules.First(y => y.Name.Equals(type)).Enable();
                            await Context.Message.AddReactionAsync(new Emoji("\U0001f44c")).ConfigureAwait(false);
                        }
                        else await Context.Message.AddReactionAsync(new Emoji("\U000026a0")).ConfigureAwait(false);
                        break;
                    case "stop":
                        if (!JobManager.AllSchedules.First(y => y.Name.Equals(type)).Disabled)
                        {
                            JobManager.AllSchedules.First(y => y.Name.Equals(type)).Disable();
                            await Context.Message.AddReactionAsync(new Emoji("\U0001f44c")).ConfigureAwait(false);
                        }
                        else await Context.Message.AddReactionAsync(new Emoji("\U000026a0")).ConfigureAwait(false);
                        break;
                    case "status":
                        if (JobManager.AllSchedules.First(x => x.Name.Equals(type)).Disabled)
                            await Context.Message.AddReactionAsync(new Emoji("\U0001f6d1")).ConfigureAwait(false);
                        else await Context.Message.AddReactionAsync(new Emoji("\U0001f501")).ConfigureAwait(false);
                        break;
                    case "next":
                        var eb = new EmbedBuilder();
                        if (!JobManager.AllSchedules.First(x => x.Name.Equals(type)).Disabled)
                        {
                            eb.WithSuccessColor()
                                .WithDescription($"**Next run:** {JobManager.AllSchedules.First(x => x.Name.Equals(type)).NextRun:g}");
                        }
                        else
                        {
                            eb.WithErrorColor()
                                .WithDescription("This scheduler is currently disabled.");
                        }
                        await Context.Channel.SendMessageAsync(string.Empty, embed: eb.Build());
                        break;
                    default:
                        await Context.Message.AddReactionAsync(new Emoji("\U000026a0")).ConfigureAwait(false);
                        break;
                }
            }
        }
        
    }
}
