namespace BackpackBot.Services.Database.Extensions
{
    using SQLite;
    using System.Collections.Generic;

    public static class DbExtensions 
    {
        public static (int, int) UpdateAndInsert<T>(this SQLiteConnection conn, List<T> items)
        {
            int updated = 0, inserted = 0;
            updated = conn.UpdateAll(items);
            inserted = conn.InsertAll(items, "OR IGNORE");
            return (updated, inserted);
        }
    }
}
