using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class TipsPurchaseDbContext : DbContext
    {

        public TipsPurchaseDbContext(DbContextOptions<TipsPurchaseDbContext> options) : base(options)
        {

        }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; }
        public DbSet<PoItem> PoItems { get; set; }

    }
}
