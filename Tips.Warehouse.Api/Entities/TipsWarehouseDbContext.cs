using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Entities
{
    public class TipsWarehouseDbContext : DbContext
    {

        public TipsWarehouseDbContext(DbContextOptions<TipsWarehouseDbContext> options) : base(options)
        { 
        }
        
        public DbSet<OpenDeliveryOrder> OpenDeliveryOrders { get; set; }
        public DbSet<OpenDeliveryOrderParts> OpenDeliveryOrderParts { get; set; }
        public DbSet<Invoice> invoices { get; set; }
        public DbSet<InvoiceChildItem> invoiceChildItems { get; set; }
        public DbSet<BTODeliveryOrder> bTODeliveryOrder { get; set; }
        public DbSet<BTODeliveryOrderItems> bTODeliveryOrderItems { get; set; }

        public DbSet<DeliveryOrder> DeliveryOrder { get; set; }
        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<InventoryTranction> InventoryTranctions { get; set; }
        public DbSet<ReturnBtoDeliveryOrder> ReturnBtoDeliveryOrders { get; set; }
        public DbSet<ReturnBtoDeliveryOrderItems> ReturnBtoDeliveryOrderItems { get; set; }
        public DbSet<ReturnInvoice> ReturnInvoices { get; set; }
        public DbSet<ReturnInvoiceItem> ReturnInvoiceItems { get; set; }
        public DbSet<DeliveryOrderTransaction> DeliveryOrderTransactions { get; set; }
        public DbSet<BTODeliveryOrderHistory> BTODeliveryOrderHistories { get; set; }


    }
}
