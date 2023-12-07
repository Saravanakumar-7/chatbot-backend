using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Entities
{
    public class TipsWarehouseDbContext : DbContext
    {

        public TipsWarehouseDbContext(DbContextOptions<TipsWarehouseDbContext> options) : base(options)
        { 
        }
        public DbSet<ReturnOpenDeliveryOrderItemQtyDistribution> ReturnOpenDeliveryOrderItemQtyDistribution { get; set; }
        public DbSet<ReturnDeliveryOrderItemQtyDistribution> ReturnDeliveryOrderItemQtyDistribution { get; set; }
        public DbSet<BtoDeliveryOrderItemQtyDistribution> BtoDeliveryOrderItemQtyDistribution { get; set; }
        public DbSet<OpenDeliveryOrderPartsQtyDistribution> OpenDeliveryOrderPartsQtyDistribution { get; set; }
        public DbSet<OpenDeliveryOrder> OpenDeliveryOrders { get; set; }
        public DbSet<OpenDeliveryOrderParts> OpenDeliveryOrderParts { get; set; }
        public DbSet<OpenDeliveryOrderHistory> OpenDeliveryOrderHistories { get; set; }
        public DbSet<ReturnOpenDeliveryOrder> ReturnOpenDeliveryOrders { get; set; }
        public DbSet<ReturnOpenDeliveryOrderParts> ReturnOpenDeliveryOrderParts { get; set; }
        public DbSet<ODONumber> ODONumbers { get; set; }
        public DbSet<Invoice> invoices { get; set; }
        public DbSet<InvoiceChildItem> invoiceChildItems { get; set; }
        public DbSet<BTODeliveryOrder> bTODeliveryOrder { get; set; }
        public DbSet<BTODeliveryOrderItems> bTODeliveryOrderItems { get; set; }
        public DbSet<BTONumber> BTONumbers { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrder { get; set; }
        public DbSet<DONumber> DONumbers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InvoiceNumber> InvoiceNumbers { get; set; }
        public DbSet<InventoryTranction> InventoryTranctions { get; set; }
        public DbSet<ReturnBtoDeliveryOrder> ReturnBtoDeliveryOrders { get; set; }
        public DbSet<ReturnBtoDeliveryOrderItems> ReturnBtoDeliveryOrderItems { get; set; }
        public DbSet<ReturnInvoice> ReturnInvoices { get; set; }
        public DbSet<ReturnInvoiceItem> ReturnInvoiceItems { get; set; }
        public DbSet<DeliveryOrderTransaction> DeliveryOrderTransactions { get; set; }
        public DbSet<ShopOrderMaterialIssueTracker> ShopOrderMaterialIssueTrackers { get; set; }
        public DbSet<BTODeliveryOrderHistory> BTODeliveryOrderHistories { get; set; }

        public DbSet<InvoiceAdditionalCharges> InvoiceAdditionalCharges { get; set; }

        public DbSet<LocationTransfer> locationTransfers { get; set; }
        //public DbSet<ConsumptionReport> ConsumptionReport { get; set; }

        public DbSet<ConsumptionReport> ConsumptionReport { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsumptionReport>().HasNoKey();

            // Other entity configurations can be added here

            base.OnModelCreating(modelBuilder);
        }



    }
}
