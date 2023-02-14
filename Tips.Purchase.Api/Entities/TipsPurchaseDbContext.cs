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
        public DbSet<PoAddProject> PoAddProjects { get; set; }
        public DbSet<PoAddDeliverySchedule> PoAddDeliverySchedules { get; set; }
        public DbSet<PrItem> PrItems { get; set; }
        public DbSet<PrAddProject> PrAddProjects { get; set; }
        public DbSet<PrAddDeliverySchedule> PrAddDeliverySchedules { get; set; }

        public DbSet<DocumentUpload> DocumentUploads { get; set; }  


    }
}
