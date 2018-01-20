using System;
using System.Collections.Generic;
using System.Text;

namespace BackpackBot.Services
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using Discord;
    using Newtonsoft.Json.Linq;
    using NLog;

    public class BotConfig
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private readonly string configFilename = Path.Combine(Directory.GetCurrentDirectory(), "Data/BotConfig.json");

        public BotConfig()
        {
            if (!File.Exists(configFilename))
            {
                log.Fatal($"Could not load bot configuration file from source \"{configFilename}\". Edit BotConfig_example.json in a text editor, rename it to Botconfig.json, then restart the bot.");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            try
            {
                var data = JObject.Parse(File.ReadAllText(configFilename));

                BotToken = data.SelectToken("BotToken").ToString();
                if (string.IsNullOrWhiteSpace(BotToken))
                {
                    log.Fatal("BotToken is missing from BotConfig.json. Add it and restart the bot.");
                    Console.ReadKey();
                    Environment.Exit(-2);
                }

                BackpackApiKey = data.SelectToken("BackpackApiKey").ToString();
                SteamApiKey = data.SelectToken("SteamApiKey").ToString();
                SuccessColor = new Color(Convert.ToUInt32(data.SelectToken("SuccessColor").ToString(), 16));
                ErrorColor = new Color(Convert.ToUInt32(data.SelectToken("ErrorColor").ToString(), 16));
                OwnerIds = data.SelectToken("OwnerIds").Children().Select(c => ulong.Parse(c.ToString())).ToList();
            }
            catch (Exception ex)
            {
                log.Fatal(ex, ex.Message, null);
                throw;
            }
        }

        public string BotToken { get; private set; }

        public string BackpackApiKey { get; private set; }

        public string SteamApiKey { get; private set; }

        public Color SuccessColor { get; private set; }

        public Color ErrorColor { get; private set; }
        
        public IReadOnlyList<ulong> OwnerIds { get; private set; }

        public bool IsOwner(IUser u)
            => OwnerIds.Contains(u.Id);
    }
}
