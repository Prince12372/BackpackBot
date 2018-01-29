namespace BackpackBot.Modules.Searches.Services
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ArgumentParser
    {
        private static Dictionary<string, string> longArgTypes = new Dictionary<string, string>()
        {
            { "--quality", "quality" },
            { "--effect", "priceindex" },
            { "--series", "priceindex" }
        };

        private static Dictionary<string, string> shortArgTypes = new Dictionary<string, string>()
        {
            { "-q", "quality" },
            { "-e", "priceindex" },
            { "-s", "priceindex" }
        };

        public static List<Argument> ParseArgs(params string[] args)
        {
            List<string> arguments = args.ToList();
            List<Argument> allArgs = new List<Argument>();

            foreach (string s in arguments)
            {
                List<string> temp = null;
                Argument a = null;
                if (s.StartsWith("--") && longArgTypes.ContainsKey(s))
                {
                    temp = args.TakeWhile(x => arguments.IndexOf(x) > 0 && !x.StartsWith('-')).ToList();
                    a = ParseLongArg(temp);
                }
                else if (s.StartsWith('-') && shortArgTypes.ContainsKey(s))
                {
                    temp = args.TakeWhile(x => arguments.IndexOf(x) > 0 && !x.StartsWith('-')).ToList();
                    a = ParseShortArg(temp);
                }
                allArgs.Add(a);
                arguments = arguments.TakeLast(arguments.Count - temp.Count).ToList();
            }

            return allArgs;
        }

        private static Argument ParseLongArg(List<string> longArg)
            => new Argument()
            {
                Name = longArgTypes[longArg[0]],
                Data = string.Join(' ', longArg.TakeLast(longArg.Count - 1))
            };

        private static Argument ParseShortArg(List<string> shortArg)
            => new Argument()
            {
                Name = shortArgTypes[shortArg[0]],
                Data = string.Join(' ', shortArg.TakeLast(shortArg.Count - 1))
            };
    }

    public class Argument
    {
        public string Name { get; set; }

        public string Data { get; set; }
    }
}
