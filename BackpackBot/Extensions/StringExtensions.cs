namespace BackpackBot.Extensions
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System;

    public static class StringExtensions
    {
        // The behemoth.
        public static List<string> GetBestMatchesFor(List<string> toCompare, string query, int numResults)
        {
            SortedDictionary<string, int> comparisonScores = new SortedDictionary<string, int>();

            foreach (string s in toCompare)
            {
                int score = query.Length;

                if (s.ToLower().Contains(query.ToLower()))
                    score -= 2 * query.Length;

                foreach (string t in query.Split(' '))
                {
                    if (s.ToLower().Contains(t.ToLower()))
                        score -= t.Length;
                }

                score += ComputeLevenshteinDistance(query, s);
                comparisonScores.Add(s, score);
            }

            var sorted = comparisonScores.OrderBy(c => c.Value);

            return sorted.ToDictionary(x => x.Key, x => x.Value).Keys.Take(numResults).ToList();
        }

        private static int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        private static string Sanitize(string input)
        {
            StringBuilder sb = new StringBuilder();
            input = input.ToLower();

            // if the char is 0-9 or a-z, include it
            foreach (char c in input)
            {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z'))
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
