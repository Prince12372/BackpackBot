using System;
using System.Collections.Generic;
using System.Text;

namespace BackpackBot.Services.Database.Models
{
    using SQLite;

    [Table("SpecialItems")]
    public class DbSpecialItem
    {
        [PrimaryKey]
        [NotNull]
        public string InternalName { get; set; }

        [NotNull]
        public string ItemName { get; set; }

        [NotNull]
        public long DefIndex { get; set; }

        [NotNull]
        public string Description { get; set; }

        [NotNull]
        public int AppId { get; set; }
    }
}
