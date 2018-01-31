namespace BackpackBot.Modules.Searches.Common.ItemSearch
{
    using CommandLine;

    public class ItemSearchOptions
    {
        [Option('q', "quality", Required = false, Default = "Genuine", HelpText = "The quality of the item (Genuine, Strange, Unusual)")]
        public string Quality { get; set; } = "Genuine";

        [Option('p', "priceindex", Required = false, Default = "0", HelpText = "The price index for the item - can be an effect name or crate series")]
        public string PriceIndex { get; set; } = "0";

        [Option('c', "craftable", Required = false, Default = true, HelpText = "Whether to search for the Craftable or Non-Craftable variant of an item")]
        public bool Craftable { get; set; } = true;

        [Option('a', "australium", Required = false, Default = false, HelpText = "Whether to search for the Australium variant of a weapon (if it exists)")]
        public bool Australium { get; set; } = false;
    }
}
