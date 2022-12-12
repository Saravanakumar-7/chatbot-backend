using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class TipsPurchaseDbContext : DbContext
    {

        public TipsPurchaseDbContext(DbContextOptions<TipsPurchaseDbContext> options) : base(options)
        {

        }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseRequisition> PurchaseRequisition { get; set; }

    }
}
