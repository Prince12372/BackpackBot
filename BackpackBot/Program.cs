namespace BackpackBot
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using BackpackBot.Extensions;
    using BackpackBot.Services;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Scheduler;
    using BackpackWebAPI;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using FluentScheduler;
    using Microsoft.Extensions.DependencyInjection;
    using NLog;

    public class Program
    {
        private BotConfig config;
        private Logger log;
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            config = new BotConfig();
            Logging.SetupLogger();
            log = LogManager.GetCurrentClassLogger();
            DbService.SetupDb();
            DbSchedulerService.Initialize();

            // connect to and login to Discord
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().AddSingleton(client).AddSingleton(commands).BuildServiceProvider();

            log.Info("Installing commands...");
            await InstallCommandsAsync().ConfigureAwait(false);
            log.Info("BackpackBot Logging in...");
            await client.LoginAsync(TokenType.Bot, config.BotToken).ConfigureAwait(false);
            log.Info("Starting bot.");
            await client.StartAsync().ConfigureAwait(false);

            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly()).ConfigureAwait(false);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var watch = Stopwatch.StartNew();
            var message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            int argPos = 0;

            if (!(message.HasStringPrefix("bp!", ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
            {
                return;
            }

            var context = new SocketCommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, services).ConfigureAwait(false);
            watch.Stop();
            log.Info(
                $"\n         Server: {context.Guild.Name + " [" + context.Guild.Id + "]"}" +
                $"\n         Channel: {context.Channel.Name + " [" + context.Channel.Id + "]"}" +
                $"\n         User: {context.Message.Author + " [" + context.Message.Author.Id + "]"}" +
                $"\n         Command: {context.Message.Content}" +
                $"\n         Execution Time: {watch.ElapsedMilliseconds / 1000.0}s");

            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(
                    string.Empty,
                    embed: new EmbedBuilder()
                        .WithErrorColor()
                        .AddField("Command error:", "```\n" + result.ErrorReason + "\n```")
                        .Build());
            }
        }
    }
}
