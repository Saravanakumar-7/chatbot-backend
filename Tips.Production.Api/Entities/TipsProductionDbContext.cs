using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Entities
{
    public class TipsProductionDbContext : DbContext
    {

        public TipsProductionDbContext(DbContextOptions<TipsProductionDbContext> options) : base(options)
        {

        }

        public DbSet<ShopOrder> ShopOrders { get; set; }

        public DbSet<ShopOrderItem> ShopOrderItems { get; set; }

        public DbSet<ShopOrderConfirmation> shopOrderConfirmations { get; set; }

        public DbSet<SAShopOrder> SAshopOrders { get; set; }

        public DbSet<SAShopOrderMaterialIssue> SAShopOrderMaterialIssues { get; set; }

        public DbSet<FGShopOrderMaterialIssue> FGShopOrderMaterialIssues { get; set; }
        public DbSet<MaterialReturnNote> MaterialReturnNotes { get; set; }
    }
}
