  using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Entities
{
    public class TipsSalesServiceDbContext : DbContext
    {
        
        public TipsSalesServiceDbContext(DbContextOptions<TipsSalesServiceDbContext> options) : base(options)
        {

        }
       
        public DbSet<Rfq> Rfqs { get; set; }
        public DbSet<QuoteEmailsDetails> QuoteEmailsDetails { get; set; }
        public DbSet<SalesOrderEmailsDetails> SalesOrderEmailsDetails { get; set; }
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
        public DbSet<ScheduleDateHistory> ScheduleDateHistories { get; set; }
        public DbSet<SalesOrderAdditionalChargesHistory> SalesOrderAdditionalChargesHistories { get; set; }
        public DbSet<SONumber> SONumbers { get; set; }
        public DbSet<RecievableCustomer> RecievableCustomers { get; set; }

        public DbSet<CollectionTracker> CollectionTrackers { get; set; }
        public DbSet<SOBreakDown> SOBreakDowns { get; set; }

        public DbSet<CoverageReport> CoverageReports { get; set; }
        public DbSet<SoConfirmationDate> SoConfirmationDates { get; set; }
        public DbSet<SoConfirmationDateHistory> SoConfirmationDateHistories { get; set; }
        public DbSet<SalesOrderSPReport> SalesOrderSPResports { get; set; }
        public DbSet<CollectionTrackerSPReport> CollectionTrackerSPReports { get; set; }
        public DbSet<CollectionTrackerByCustomerIdSPReport> CollectionTrackerByCustomerIdSPReports { get; set; }
        public DbSet<CollectionTrackerBySalesOrderNoSPReport> CollectionTrackerBySalesOrderNoSPReports { get; set; }
        public DbSet<DocumentUpload> DocumentUploads { get; set; }
        public DbSet<SOSummarySPReport> SOSummarySPReports { get; set; }
        public DbSet<SOMonthlyConsumptionSPReport> SOMonthlyConsumptionSPReports { get; set; }
        public DbSet<CollectionTrackerWithCustomerWiseSPReport> CollectionTrackerWithCustomerWiseSPReports { get; set; }
        public DbSet<CollectionTrackerWithSalesOrderNoWiseSPReport> CollectionTrackerWithSalesOrderNoWiseSPReports { get; set; }
        public DbSet<RfqSalesOrderSPReport> RfqSalesOrderSPReports { get; set; }
        public DbSet<ForecastSalesOrderSPReport> ForecastSalesOrderSPReports { get; set; }
        public DbSet<RfqSPReport> RfqSPReports { get; set; }
        public DbSet<RfqSPReportForTras> RfqSPReportForKeus { get; set; }
        public DbSet<QuoteSPReport> QuoteSPReports { get; set; }
        public DbSet<RfqSalesOrderSPReportForTrans> RfqSalesOrderSPReportsForTrans { get; set; }
        public DbSet<ForecastSalesOrderSPReportForTrans> ForecastSalesOrderSPReportsForTrans { get; set; }
        public DbSet<LPCostingSPReport> LPCostingSPReport { get; set; }
        public DbSet<LPCostingforFGSPReport> LPCostingforFGSPReport { get; set; }
        public DbSet<LPCostingSummarySPReport> LPCostingSummarySPReport { get; set; }
        public DbSet<RfqSalesOrderRoomWiseSPReport> RfqSalesOrderRoomWiseSPReports { get; set; }
        public DbSet<QuotationSPReport> QuotationSPReports { get; set; }
        public DbSet<SalesOrderMainLevelHistory> SalesOrderMainLevelHistories { get; set; }
        public DbSet<SOAdditionalChargesHistory> SOAdditionalChargesHistories { get; set; }
        public DbSet<SalesOrderItemLevelHistory> SalesOrderItemLevelHistories { get; set; }
        public DbSet<SalesOrderScheduleDateHistory> SalesOrderScheduleDateHistories { get; set; }
        public DbSet<SalesOrderMainLevelHistorySP> SalesOrderMainLevelHistorySP { get; set; }
        public DbSet<SalesOrderDashboardSPReport> SalesOrderDashboardSPReports { get; set; }
        public DbSet<TransactionDashboardSPReport> TransactionDashboardSPReports { get; set; }
        public DbSet<FinancialYearDashboardSPReport> FinancialYearDashboardSPReports { get; set; }
        public DbSet<SourcingSPReport> SourcingSPReports { get; set; }
        public DbSet<SOInitialConfirmationDateHistory> SOInitialConfirmationDateHistories { get; set; }
        public DbSet<FGSalesOrderSPReport> FGSalesOrderSPReports { get; set; }
        public DbSet<SalesOrderQtyDetailsDto> SalesOrderQtyDetailsDtos { get; set; }
        public DbSet<SalesServiceFileUpload> SalesServiceFileUpload { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecievableCustomer>().HasNoKey();
            modelBuilder.Entity<SalesOrderSPReport>().HasNoKey();
            modelBuilder.Entity<CollectionTrackerSPReport>().HasNoKey();
            modelBuilder.Entity<CollectionTrackerByCustomerIdSPReport>().HasNoKey();
            modelBuilder.Entity<CollectionTrackerBySalesOrderNoSPReport>().HasNoKey();
            modelBuilder.Entity<SOSummarySPReport>().HasNoKey();
            modelBuilder.Entity<SOMonthlyConsumptionSPReport>().HasNoKey();
            modelBuilder.Entity<CollectionTrackerWithCustomerWiseSPReport>().HasNoKey();
            modelBuilder.Entity<CollectionTrackerWithSalesOrderNoWiseSPReport>().HasNoKey();
            modelBuilder.Entity<RfqSalesOrderSPReport>().HasNoKey();
            modelBuilder.Entity<ForecastSalesOrderSPReport>().HasNoKey();
            modelBuilder.Entity<RfqSPReportForTras>().HasNoKey();
            modelBuilder.Entity<RfqSPReport>().HasNoKey();
            modelBuilder.Entity<QuoteSPReport>().HasNoKey();
            modelBuilder.Entity<RfqSalesOrderSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<ForecastSalesOrderSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<LPCostingSPReport>().HasNoKey();
            modelBuilder.Entity<LPCostingforFGSPReport>().HasNoKey();
            modelBuilder.Entity<LPCostingSummarySPReport>().HasNoKey();
            modelBuilder.Entity<CommoditySourcingSpReport>().HasNoKey();
            modelBuilder.Entity<VendorSourcingSpReport>().HasNoKey();
            modelBuilder.Entity<QuoteforKeusDto>().HasNoKey();
            modelBuilder.Entity<SalesOrderforKeusDto>().HasNoKey();
            modelBuilder.Entity<QuotationSPReport>().HasNoKey();
            modelBuilder.Entity<RfqSalesOrderRoomWiseSPReport>().HasNoKey();
            modelBuilder.Entity<SoSummaryQuotationDto>().HasNoKey();
            modelBuilder.Entity<CustomerWiseTransactionSPReport>().HasNoKey();
            modelBuilder.Entity<SalesOrderId_SP>().HasNoKey();
            modelBuilder.Entity<RFQSalesorderConfirmationSPReport>().HasNoKey();
            modelBuilder.Entity<SalesOrderDashboardSPReport>().HasNoKey();
            modelBuilder.Entity<TransactionDashboardSPReport>().HasNoKey();
            modelBuilder.Entity<FinancialYearDashboardSPReport>().HasNoKey();
            modelBuilder.Entity<TransactionDashboardSPReport_bucketId1>().HasNoKey();
            modelBuilder.Entity<TransactionDashboardSPReport_bucketId2>().HasNoKey();
            modelBuilder.Entity<TransactionDashboardSPReport_bucketId3>().HasNoKey();
            modelBuilder.Entity<TransactionDashboardSPReport_bucketId5>().HasNoKey();
            modelBuilder.Entity<SourcingSPReport>().HasNoKey();
            modelBuilder.Entity<FGSalesOrderSPReport>().HasNoKey();
            modelBuilder.Entity<SalesOrderQtyDetailsDto>().HasNoKey();
            modelBuilder.Entity<FGSalesOrderSPReportWithDate>().HasNoKey();
            modelBuilder.Entity<RecievableDayWiseSPReportDto>().HasNoKey();
            modelBuilder.Entity<SalesOrderDetailsTOSDto>().HasNoKey();
            modelBuilder.Entity<SOLeadWiseDataSPReport>().HasNoKey();
            modelBuilder.Entity<FQToFSSPReport>().HasNoKey();
            modelBuilder.Entity<SalesOrderSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<AdvanceRecievableSPReportDto>().HasNoKey();
            modelBuilder.Entity<InitialAdvanceCustomerSPReport>().HasNoKey();
            modelBuilder.Entity<AdvanceReceivedEntryLevelSPResport>().HasNoKey();
            modelBuilder.Entity<FirstAdvanceReceivedEntryLevelSPResport>().HasNoKey();
            modelBuilder.Entity<LatestAdvanceReceivedEntryLevelSPResport>().HasNoKey();
            modelBuilder.Entity<QuoteRevNoSPReportParam>().HasNoKey();
            modelBuilder.Entity<SalesRevNoSPReportParam>().HasNoKey();
            modelBuilder.Entity<FQToFSFirstSOSPReport>().HasNoKey();
            modelBuilder.Entity<FQToFSFirstQuoteSPReport>().HasNoKey();
            modelBuilder.Entity<FQToFSFirstQuoteSentSPReport>().HasNoKey();
            modelBuilder.Entity<FQToFSLatestSOSPReport>().HasNoKey();
            
        }

    }
}
