using CommonModels;
using Microsoft.EntityFrameworkCore;

namespace Tips.Master.Api.Data
{
    public class TipsMasterDbContext : DbContext
    {
        public TipsMasterDbContext(DbContextOptions<TipsMasterDbContext> options) : base(options)   
        {

        }

        DbSet<CustomerType> CustomerTypes;
        DbSet<LeadTime> LeadTimes;
        DbSet<MaterialType> MaterialTypes;
        DbSet<ProcurementType> ProcurementTypes;

    }
}
