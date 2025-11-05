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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TallyVendorMasterSpReport>().HasNoKey();

        }
    }
}
