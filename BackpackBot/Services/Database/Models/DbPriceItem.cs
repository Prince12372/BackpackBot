namespace BackpackBot.Services.Database.Models
{
    using SQLite;

    [Table("CommunityPrices")]
    public class DbPriceItem
    {
        [NotNull]
        [Indexed(Name = "PriceIndex", Order = 1)]
        public long DefIndex { get; set; }

        [NotNull]
        public string Name { get; set; }

        [Indexed(Name = "PriceIndex", Order = 2)]
        public string Quality { get; set; }

        [NotNull]
        [Indexed(Name = "PriceIndex", Order = 3)]
        public string Craftability { get; set; }

        [Indexed(Name = "PriceIndex", Order = 4)]
        public string EffectOrSeries { get; set; }

        [Indexed(Name = "PriceIndex", Order = 5)]
        public bool? Australium { get; set; }

        public string Currency { get; set; }

        public double? Value { get; set; }

        public double? HighValue { get; set; }

        public string LastUpdate { get; set; }

        public double? Difference { get; set; }
    }
}
