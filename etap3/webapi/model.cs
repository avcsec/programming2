using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace etap2
{
    public class HistoryContext : DbContext
    {
        public DbSet<History> Histories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=database.sqlite");
    }

    public class History
    {
        public int HistoryId { get; set; }
        public string formula { get; set; }
        public double? x { get; set; }
        public double? from { get; set; }
        public double? to { get; set; }
        public int? n { get; set; }
    }
}