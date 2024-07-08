using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Entities
{
    public class TipsWarehouseDbContext : DbContext
    {

        public TipsWarehouseDbContext(DbContextOptions<TipsWarehouseDbContext> options) : base(options)
        {
        }
        public DbSet<ReturnInvoiceItemQtyDistribution> ReturnInvoiceItemQtyDistribution { get; set; }
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
        public DbSet<DeliveryOrderSPReport> RecievableBTOs { get; set; }
        public DbSet<InvoiceSPReport> RecievableInvoices { get; set; }
        public DbSet<LocationTransferSPReport> RecievableLocationTransfers { get; set; }
        public DbSet<MRNSPReport> MRNSPReports { get; set; }        
        public DbSet<OpenDeliveryOrderSPReport> RecievableODOs { get; set; }
        public DbSet<DailyDOReport> DailyDOReports { get; set; }
        public DbSet<ReturnInvoiceSPResport> ReturnInvoiceSPResports { get; set; }
        public DbSet<ReturnOpenDeliveryOrderSPResport> ReturnOpenDeliveryOrderSPResports { get; set; }
        public DbSet<ReturnDOSPReport> ReturnDOSPReports { get; set; }
        public DbSet<InventorySPReport> InventorySPReports { get; set; }
        public DbSet<ODOMonthlyConsumptionSPReport> ODOMonthlyConsumptionSPReports { get; set; }
        public DbSet<InventoryTranctionSPReport> InventoryTranctionSPReports { get; set; }
        public DbSet<CrossMarginSPReport> CrossMarginSPReports { get; set; }
        public DbSet<StockMovementSPReport> StockMovementSPReports { get; set; }
        public DbSet<InventoryForStockSPReport> InventoryForStockSPReports { get; set; }
        public DbSet<InvoiceForTransSPReport> InvoiceForTransSPReports { get; set; }
        public DbSet<StockMovementLatestSPReport> CustomerMasterLeadIdSPReports { get; set; }
        public DbSet<TrascationKPNWSPReport> TrascationKPNWSPReports { get; set; }
        public DbSet<ReturnInvoiceAdditionalCharge> ReturnInvoiceAdditionalCharges { get; set; }
        public DbSet<StockMovementHistorySPReport> StockMovementHistorySPReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsumptionReport>().HasNoKey();
            modelBuilder.Entity<DeliveryOrderSPReport>().HasNoKey();
            modelBuilder.Entity<InvoiceSPReport>().HasNoKey();
            modelBuilder.Entity<LocationTransferSPReport>().HasNoKey();
            modelBuilder.Entity<OpenDeliveryOrderSPReport>().HasNoKey();
            modelBuilder.Entity<ReturnInvoiceSPResport>().HasNoKey();
            modelBuilder.Entity<ReturnOpenDeliveryOrderSPResport>().HasNoKey();
            modelBuilder.Entity<ReturnDOSPReport>().HasNoKey();
            modelBuilder.Entity<MRNSPReport>().HasNoKey();
            modelBuilder.Entity<InventorySPReport>().HasNoKey();
            modelBuilder.Entity<ODOMonthlyConsumptionSPReport>().HasNoKey();
            modelBuilder.Entity<InventoryTranctionSPReport>().HasNoKey();
            modelBuilder.Entity<CrossMarginSPReport>().HasNoKey();
            modelBuilder.Entity<StockMovementSPReport>().HasNoKey();
            modelBuilder.Entity<InventoryForStockSPReport>().HasNoKey();
            // Other entity configurations can be added here
            modelBuilder.Entity<DailyDOReport>().HasNoKey();
            modelBuilder.Entity<InvoiceForTransSPReport>().HasNoKey();
            modelBuilder.Entity<StockMovementLatestSPReport>().HasNoKey();
            modelBuilder.Entity<TrascationKPNWSPReport>().HasNoKey();
            modelBuilder.Entity<StockMovementHistorySPReport>().HasNoKey();
            modelBuilder.Entity<MRNSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<OpenDeliveryOrderSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<DeliveryOrderSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<InvoiceSPReportForTrans>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
        }
        


    }
}
