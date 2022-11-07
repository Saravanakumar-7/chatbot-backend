using Microsoft.EntityFrameworkCore;
using Tips.Model;

namespace Tips.Data
{
    public class TipsDBContext:DbContext
    {
        public TipsDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ItemMaster> ItemMasters { get; set; }
    }
}
