using Entities;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class TipsProductionDbContext : DbContext
    {

        public TipsProductionDbContext(DbContextOptions<TipsProductionDbContext> options) : base(options)
        {

        }

        public DbSet<ShopOrder> shopOrders { get; set; }

        public DbSet<ShopOrderConfirmation> shopOrderConfirmations { get; set; }

    }
}
