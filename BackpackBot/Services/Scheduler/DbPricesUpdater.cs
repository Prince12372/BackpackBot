namespace BackpackBot.Services.Scheduler
{
    using System;
    using System.Collections.Generic;
    using BackpackBot.Services.Database;
    using BackpackBot.Services.Database.Models;
    using BackpackWebAPI;
    using BackpackWebAPI.Models;
    using FluentScheduler;
    using NLog;

    public class DbPricesUpdater : IJob
    {
        private static BotConfig config = new BotConfig();
        private static Logger log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, string> unusualEffects = new Dictionary<string, string>()
        {
            { "4", "Community Sparkle" },
            { "5", "Holy Glow" },
            { "6", "Green Confetti" },
            { "7", "Purple Confetti" },
            { "8", "Haunted Ghosts" },
            { "9", "Green Energy" },
            { "10", "Purple Energy" },
            { "11", "Circling TF Logo" },
            { "12", "Massed Flies" },
            { "13", "Burning Flames" },
            { "14", "Scorching Flames" },
            { "17", "Sunbeams" },
            { "20", "Map Stamps" },
            { "29", "Stormy Storm" },
            { "33", "Orbiting Fire" },
            { "34", "Bubbling" },
            { "35", "Smoking" },
            { "36", "Steaming" },
            { "38", "Cloudy Moon" },
            { "56", "Kill-a-Watt" },
            { "57", "Terror-Watt" },
            { "58", "Cloud 9" },
            { "70", "Time Warp" },
            { "15", "Searing Plasma" },
            { "16", "Vivid Plasma" },
            { "18", "Circling Peace Sign" },
            { "19", "Circling Heart" },
            { "30", "Blizzardy Storm" },
            { "31", "Nuts n' Bolts" },
            { "32", "Orbiting Planets" },
            { "37", "Flaming Lantern" },
            { "39", "Cauldron Bubbles" },
            { "40", "Eerie Orbiting Fire" },
            { "43", "Knifestorm" },
            { "44", "Misty Skull" },
            { "45", "Harvest Moon" },
            { "46", "It's A Secret To Everybody" },
            { "47", "Stormy 13th Hour" },
            { "59", "Aces Highs" },
            { "60", "Dead Presidents" },
            { "61", "Miami Nights" },
            { "62", "Disco Beat Down" },
            { "63", "Phosphorous" },
            { "64", "Sulphurous" },
            { "65", "Memory Leak" },
            { "66", "Overclocked" },
            { "67", "Electrostatic" },
            { "68", "Power Surge" },
            { "69", "Anti-Freeze" },
            { "71", "Green Black Hole" },
            { "72", "Roboactive" },
            { "73", "Arcana" },
            { "74", "Spellbound" },
            { "75", "Chiroptera" },
            { "76", "Poisoned Shadows" },
            { "77", "Something Burning This Way Comes" },
            { "78", "Hellfire" },
            { "79", "Darkblaze" },
            { "80", "Demonflame" },
            { "3001", "Showstopper" },
            { "3003", "Holy Grail" },
            { "3004", "'72" },
            { "3005", "Fountain of Delight" },
            { "3006", "Screaming Tiger" },
            { "3007", "Skill Gotten Gains" },
            { "3008", "Midnight Whirlwind" },
            { "3009", "Silver Cyclone" },
            { "3010", "Mega Strike" },
            { "81", "Bonzo The All-Gnawing" },
            { "82", "Amaranthine" },
            { "83", "Stare From Beyond" },
            { "84", "The Ooze" },
            { "85", "Ghastly Ghosts Jr" },
            { "86", "Haunted Phantasm Jr" },
            { "3011", "Haunted Phantasm" },
            { "3012", "Ghastly Ghosts" },
            { "87", "Frostbite" },
            { "88", "Molten Mallard" },
            { "89", "Morning Glory" },
            { "90", "Death at Dusk" },
            { "701", "Hot" },
            { "702", "Isotope" },
            { "703", "Cool" },
            { "704", "Energy Orb" },
            { "91", "Abduction" },
            { "92", "Atomic" },
            { "93", "Subatomic" },
            { "94", "Electric Hat Protector" },
            { "95", "Magnetic Hat Protector" },
            { "96", "Voltaic Hat Protector" },
            { "97", "Galactic Codex" },
            { "98", "Ancient Codex" },
            { "99", "Nebula" },
            { "100", "Death by Disco" },
            { "101", "It's a mystery to everyone" },
            { "102", "It's a puzzle to me" },
            { "103", "Ether Trail" },
            { "104", "Nether Trail" },
            { "105", "Ancient Eldritch" },
            { "106", "Eldritch Flame" },
            { "108", "Tesla Coil" },
            { "107", "Neutron Star" },
            { "109", "Starstorm Insomnia" },
            { "110", "Starstorm Slumber" },
            { "3015", "Infernal Flames" },
            { "3013", "Hellish Inferno" },
            { "3014", "Spectral Swirl" },
            { "3016", "Infernal Smoke" }
        };
        private readonly Dictionary<string, string> qualityNames = new Dictionary<string, string>()
        {
            { "0", "Normal" },
            { "1", "Genuine" },
            { "3", "Vintage" },
            { "5", "Unusual" },
            { "6", "Unique" },
            { "7", "Community" },
            { "8", "Valve" },
            { "9", "Self-Made" },
            { "11", "Strange" },
            { "13", "Haunted" },
            { "14", "Collector's" },
            { "15", "Decorated Weapon" }
        };
        private static DbService db = new DbService();
        private static BackpackWrapper wrapper = new BackpackWrapper(config.BackpackApiKey);

        public async void Execute()
        {
            log.Info("Update started.");
            CommunityPricesRoot root = wrapper.GetCommunityPricesAsync().Result;
            List<DbPriceItem> items = new List<DbPriceItem>();

            foreach (var item in root.Response.Items)
            {
                //log.Info($"Processing item {item.Key}");
                foreach (long defindex in item.Value.DefIndexes)
                {
                    foreach (var quality in item.Value.Qualities)
                    {
                        Dictionary<string, IReadOnlyDictionary<string, ItemPrice>> craftabilities = new Dictionary<string, IReadOnlyDictionary<string, ItemPrice>>();
                        if (quality.Value.Tradable.Craftable != null)
                        {
                            craftabilities.Add("Craftable", quality.Value.Tradable.Craftable);
                        }
                        if (quality.Value.Tradable.NonCraftable != null)
                        {
                            craftabilities.Add("Non-Craftable", quality.Value.Tradable.NonCraftable);
                        }
                        foreach (var craftability in craftabilities)
                        {
                            foreach (var priceIndexKvp in craftability.Value)
                            {

                                items.Add(new DbPriceItem
                                    {
                                        DefIndex = defindex,
                                        Name = item.Key,
                                        Quality = qualityNames.ContainsKey(quality.Key) ? qualityNames[quality.Key] : null,
                                        Craftability = craftability.Key,
                                        EffectOrSeries = (item.Key.Contains("Crate") || item.Key.Contains("Case")) && !item.Key.Contains("maker") && !priceIndexKvp.Key.Equals("0")
                                            ? $"Series #{priceIndexKvp.Key}"
                                            : unusualEffects.ContainsKey(priceIndexKvp.Key)
                                            ? unusualEffects[priceIndexKvp.Key]
                                            : null,
                                        Australium = priceIndexKvp.Value.Australium,
                                        Currency = priceIndexKvp.Value.CurrencyType,
                                        Value = priceIndexKvp.Value.Value,
                                        HighValue = priceIndexKvp.Value.ValueHigh,
                                        LastUpdate = (priceIndexKvp.Value.LastUpdate.HasValue)
                                            ? string.Format("{0:g}",
                                                new DateTime(1970, 1, 1, 0, 0, 0, 0)
                                                    .AddSeconds(priceIndexKvp.Value.LastUpdate.Value))
                                            : null,
                                        Difference = priceIndexKvp.Value.Difference
                                    });
                            }
                        }
                    }
                }
            }

            try
            {
                db.UpdatePriceItems(items);
            }
            catch (Exception ex)
            {
                log.Warn(ex, ex.Message, null);
            }
            log.Info("Update complete.");
        }
    }
}
