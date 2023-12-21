  using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class TipsSalesServiceDbContext : DbContext
    {
        
        public TipsSalesServiceDbContext(DbContextOptions<TipsSalesServiceDbContext> options) : base(options)
        {

        }

        public DbSet<Rfq> Rfqs { get; set; }

        public DbSet<RfqNumber> rfqNumbers { get ;set;}
        public DbSet<RfqCustomerSupport> RfqCustomerSupports { get; set; }

        public DbSet<RfqCSDeliverySchedule> RfqCSDeliverySchedules { get; set; }

        public DbSet<RfqCustomerSupportItems> RfqCustomerSupportItems { get; set; }

        public DbSet<RfqCustomerSupportNotes> RfqCustomerSupportNotes { get; set; }

        public DbSet<RfqCustomGroup> RfqCustomGroups { get; set; }

        public DbSet<SourcingVendor> sourcingVendors { get; set; }

        public DbSet<RfqSourcing> RfqSourcings { get; set; }
        public DbSet<RfqSourcingItems> RfqSourcingItems { get; set; }

        public DbSet<RfqSourcingVendor> RfqSourcingVendors { get; set; }

        public DbSet<RfqEngg> RfqEnggs { get; set; }

        public DbSet<RfqEnggItem> RfqEnggItems { get; set; }

        public DbSet<RfqEnggRiskIdentification> RfqEnggRiskIdentifications { get; set; }
        public DbSet<RFQNo> RFQNos { get; set; }
        public DbSet<RfqLPCosting> RfqLPCostings { get; set; }

        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItems> SalesOrdersItems { get; set; }
        public DbSet<SalesOrderAdditionalCharges> SalesOrderAdditionalCharges { get; set; }
        public DbSet<QuoteNumber> QuoteNumbers { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteGeneral> QuoteGenerals { get; set; }
        public DbSet<QuoteAdditionalCharges> QuoteAdditionalCharges { get; set; }
        public DbSet<QuoteRFQNotes> QuoteRFQNotes { get; set; }
        public DbSet<QuoteOtherTerms> QuoteOtherTerms { get; set; }
        public DbSet<QuoteSpecialTerms> QuoteSpecialTerms { get; set; }

        public DbSet<ForeCast> ForeCasts { get; set; }
        public DbSet<ForeCastCustomerSupport> ForeCastCustomerSupports { get; set; }
        public DbSet<ForeCastCustomerSupportItem> foreCastCustomerSupportItems { get; set; }
        public DbSet<ForeCastCSDeliverySchedule> ForeCastCSDeliverySchedules { get; set; }
        public DbSet<ForeCastCustomerSupportNotes> ForeCastCustomerSupportNotes { get; set; }
        public DbSet<ForeCastEngg> ForeCastEnggs { get; set; }
        public DbSet<ForeCastEnggItems> ForeCastEnggItems { get; set; }
        public DbSet<ForeCastEnggRiskIdentification> ForeCastEnggRiskIdentifications { get; set; }
        public DbSet<ForecastSourcing> ForecastSourcings { get; set; }
        public DbSet<ForecastSourcingItems> ForecastSourcingItems { get; set; }
        public DbSet<ForecastSourcingVendor> ForecastSourcingVendors { get; set; }
        public DbSet<ForecastNo> ForecastNos { get; set; }
        public DbSet<RfqLPCostingItem> RfqLPCostingItems { get; set; }

        public DbSet<RfqLPCostingProcess> RfqLPCostingProcesses { get; set; }
        public DbSet<RfqLPCostingNREConsumable> RfqLPCostingNREConsumables { get; set; }
        public DbSet<RfqLPCostingOtherCharges> RfqLPCostingOtherCharges { get; set; }
        public DbSet<FgOqc> FgOqcs { get; set; }
        public DbSet<FinalOqc> FinalOqcs { get; set; }
        public DbSet<ItemPriceList> ItemPriceLists { get; set; }
        public DbSet<ForecastLpCosting> ForecastLpCostings { get; set; }
        public DbSet<ForecastLpCostingItem> ForecastLpCostingItems { get; set; }
        public DbSet<ForecastLpCostingProcess> ForecastLpCostingProcesses { get; set; }
        public DbSet<ForecastLPCostingNREConsumable> ForecastLPCostingNREConsumables { get; set; }
        public DbSet<ForecastLpCostingOtherCharges> ForecastLpCostingOtherCharges { get; set; }
        public DbSet<RfqCustomField> RfqCustomFields { get; set; }
        public DbSet<ForeCastCustomGroup> ForeCastCustomGroups { get; set; }
        public DbSet<ForeCastCustomField> ForeCastCustomFields { get; set; }
        public DbSet<MRNumber> MRNumbers { get; set; }
        public DbSet<MaterialRequest> MaterialRequests { get; set; }
        public DbSet<MaterialRequestItem> MaterialRequestItems { get; set; }
        public DbSet<MaterialTransactionNote> MaterialTransactionNotes { get; set; }
        public DbSet<MaterialTransactionNoteItem> MaterialTransactionNoteItems { get; set; }
        public DbSet<LocationTransfer> locationTransfers { get; set; }
        public DbSet<ReleaseLp> ReleaseLps { get; set; } 
        public DbSet<ScheduleDate> ScheduleDates { get; set; }

        public DbSet<ForeCastReleaseLp> ForeCastReleaseLps { get; set; }

        public DbSet<SalesOrderHistory> salesOrderHistories { get; set; }
        public DbSet<SONumber> SONumbers { get; set; }
        public DbSet<RecievableCustomer> RecievableCustomers { get; set; }

        public DbSet<CollectionTracker> CollectionTrackers { get; set; }
        public DbSet<SOBreakDown> SOBreakDowns { get; set; }

        public DbSet<CoverageReport> CoverageReports { get; set; }
        public DbSet<SoConfirmationDate> SoConfirmationDates { get; set; }
        public DbSet<SoConfirmationDateHistory> SoConfirmationDateHistories { get; set; }
        public DbSet<SalesOrderSPResport> SalesOrderSPResports { get; set; }
        public DbSet<DocumentUpload> DocumentUploads { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecievableCustomer>().HasNoKey();
            modelBuilder.Entity<SalesOrderSPResport>().HasNoKey();
        }

    }
}
