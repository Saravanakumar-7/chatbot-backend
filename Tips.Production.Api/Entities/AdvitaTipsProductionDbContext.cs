using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class AdvitaTipsProductionDbContext : DbContext
    {

        public AdvitaTipsProductionDbContext(DbContextOptions<AdvitaTipsProductionDbContext> options) : base(options)
        {

        }
        public DbSet<AdvitaShopOrderDetails> shoporder { get; set; }
       
    }
}
