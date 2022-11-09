using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class TipsSalesServiceDbContext : DbContext
    {
        public TipsSalesServiceDbContext(DbContextOptions<TipsSalesServiceDbContext> options) : base(options)   
        {

        }

        public DbSet<Rfq> rfqs { get; set; }
        

    }
}
