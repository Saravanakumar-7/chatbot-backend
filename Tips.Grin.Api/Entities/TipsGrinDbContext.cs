using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class TipsGrinDbContext : DbContext
    {

        public TipsGrinDbContext(DbContextOptions<TipsGrinDbContext> options) : base(options)
        {

        }
        public DbSet<Grins> Grins { get; set; }

        public DbSet<GrinParts> GrinParts { get; set; }

        public DbSet<IQCConfirmation> IQCConfirmations { get; set; }
        public DbSet<Binning> Binnings { get; set; }
        public DbSet<BinningItems> BinningItem { get; set; }

        public DbSet<BinningLocation> BinningLocations { get; set; }



    }
}
