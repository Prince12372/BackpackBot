namespace BackpackBot.Modules.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using BackpackBot.Extensions;
    using BackpackBot.Services;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using FluentScheduler;
    using NLog;

    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        private Logger log = LogManager.GetCurrentClassLogger();
        private BotConfig config = new BotConfig();
        private static readonly IEmote left_arrow = new Emoji("\U00002b05");
        private static readonly IEmote right_arrow = new Emoji("\U000027a1");

        private List<string> types = new List<string>()
        {
            "prices",
            "currencies",
            "specials"
        };
        private List<string> actions = new List<string>()
        {
            "enable",
            "disable",
            "status"
        };

        [Command("scheduler")]
        [Alias("sched")]
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

                if (!types.Contains(type) || !actions.Contains(action))
                {
                    await Context.Message.AddReactionAsync(new Emoji("\U00002753")).ConfigureAwait(false);
                    return;
                }

                switch (action)
                {
                    case "enable":
                        if (JobManager.AllSchedules.First(y => y.Name.Equals(type)).Disabled)
                        {
                            JobManager.AllSchedules.First(y => y.Name.Equals(type)).Enable();
                            await Context.Message.AddReactionAsync(new Emoji("\U0001f44c")).ConfigureAwait(false);
                        }
                        else await Context.Message.AddReactionAsync(new Emoji("\U000026a0")).ConfigureAwait(false);
                        break;
                    case "disable":
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
