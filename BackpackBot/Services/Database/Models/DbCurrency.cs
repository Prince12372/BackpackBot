using System;
using System.Collections.Generic;
using System.Text;

namespace BackpackBot.Services.Database.Models
{
    using SQLite;

    [Table("Currencies")]
    public class DbCurrency
    {
        [PrimaryKey]
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public long RoundTo { get; set; }

        [NotNull]
        public double Value { get; set; }

        [NotNull]
        public double HighValue { get; set; }

        [NotNull]
        public string ValueType { get; set; }

        [NotNull]
        public double Difference { get; set; }

        [NotNull]
        public string LastUpdate { get; set; }
    }
}
