using Microsoft.EntityFrameworkCore;
using Tips.Tally.Api.Entities.DTOs;

namespace Tips.Tally.Api.Entities
{
    public class TipsTallyDbContext : DbContext
    {
        public TipsTallyDbContext(DbContextOptions<TipsTallyDbContext> options) : base(options)
        {

        }
        public DbSet<TallyVendorMasterSpReport> TallyVendorMasterSpReport { get; set; }
        public DbSet<TallyCurrencyMasterSPReport> TallyCurrencyMasterSPReport { get; set; }
        public DbSet<TallyPurchaseOrderSpReport> TallyPurchaseOrderSpReport { get; set; }
        public DbSet<TallyStockItemSPReport> TallyStockItemSPReport { get; set; }
        public DbSet<TallyCustomerMasterSpReport> TallyCustomerMasterSpReport { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TallyVendorMasterSpReport>().HasNoKey();
            modelBuilder.Entity<TallyCustomerMasterSpReport>().HasNoKey();
            modelBuilder.Entity<TallyPurchaseOrderSpReport>().HasNoKey();
            modelBuilder.Entity<TallyStockItemSPReport>().HasNoKey();
            modelBuilder.Entity<TallyCurrencyMasterSPReport>().HasNoKey();

        }
    }
}
