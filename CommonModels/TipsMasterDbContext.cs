using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class TipsMasterDbContext : DbContext
    {
        public TipsMasterDbContext(DbContextOptions<TipsMasterDbContext> options) : base(options)   
        {

        }

        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<LeadTime> LeadTimes { get; set; }
        public DbSet<MaterialType>? MaterialTypes { get; set; }
        public DbSet<ProcurementType>? ProcurementTypes { get; set; }

    }
}
