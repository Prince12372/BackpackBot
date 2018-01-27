namespace BackpackBot.Extensions
{
    using BackpackBot.Services;
    using Discord;

    public static class EmbedExtensions
    {
        private static BotConfig config = new BotConfig();

        public static EmbedBuilder WithErrorColor(this EmbedBuilder eb)
            => eb.WithColor(config.ErrorColor);

        public static EmbedBuilder WithSuccessColor(this EmbedBuilder eb)
            => eb.WithColor(config.SuccessColor);
    }
}
