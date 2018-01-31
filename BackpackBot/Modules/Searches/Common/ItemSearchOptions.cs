namespace BackpackBot.Modules.Searches.Common
{
    using CommandLine;
    using System.Collections.Generic;
    using System.Linq;

    public class ItemSearchOptions
    {
        // public string QualityString { get => string.Join(' ', Quality); set => value.Split(' ').ToList(); }

        [Option('q', "quality", Required = false, Default = new string[] { "Genuine" }, HelpText = "The quality of the item (Genuine, Strange, Unusual)")]
        public IEnumerable<string> Quality { get; set; } = new List<string> { "Genuine" };

        // public string PriceIndexString { get => string.Join(' ', PriceIndex); set => value.Split(' ').ToList(); }

        [Option('p', "priceindex", Required = false, Default = new string[] { "0" }, HelpText = "The price index for the item - can be an effect name or crate series")]
        public IEnumerable<string> PriceIndex { get; set; } = new List<string> { "0" };

        [Option('c', "craftable", Required = false, Default = true, HelpText = "Whether to search for the Craftable or Non-Craftable variant of an item")]
        public bool Craftable { get; set; } = true;

        [Option('a', "australium", Required = false, Default = false, HelpText = "Whether to search for the Australium variant of a weapon (if it exists)")]
        public bool Australium { get; set; } = false;
    }
}
