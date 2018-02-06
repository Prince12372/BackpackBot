using System;
using System.Collections.Generic;
using System.Text;

namespace BackpackBot.Services.Database.Models
{
    using SQLite;

    public class DbSchemaItem
    {
        [NotNull]
        [PrimaryKey]
        public uint DefIndex { get; set; }

        [NotNull]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string ImageUrlLarge { get; set; }
    }
}
