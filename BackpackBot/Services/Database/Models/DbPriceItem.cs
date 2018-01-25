namespace BackpackBot.Services.Database.Models
{
    using SQLite;

    [Table("CommunityPrices")]
    public class DbPriceItem
    {
        [PrimaryKey]
        [NotNull]
        public string UniqueId { get; set; }

        [NotNull]
        public long DefIndex { get; set; }

        [NotNull]
        public string Name { get; set; }

        public string Quality { get; set; }

        [NotNull]
        public bool Craftable { get; set; }

        public string EffectOrSeries { get; set; }

        public bool? Australium { get; set; }

        public string Currency { get; set; }

        public double? Value { get; set; }

        public double? HighValue { get; set; }

        public long? LastUpdate { get; set; }

        public double? Difference { get; set; }
    }
}
