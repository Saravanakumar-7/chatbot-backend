using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

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
        public DbSet<PONumber> PONumbers { get; set; }
        public DbSet<PRNumber> PRNumbers { get; set; }
        public DbSet<PrAddDeliverySchedule> PrAddDeliverySchedules { get; set; }

        public DbSet<DocumentUpload> DocumentUploads { get; set; }
        public DbSet<PrSpecialInstruction> PrSpecialInstructions { get; set; }
        public DbSet<PoSpecialInstruction> PoSpecialInstructions { get; set; }
        public DbSet<POCollectionTracker> POCollectionTrackers { get; set; }
        public DbSet<POBreakDown> POBreakDowns { get; set; }
        public DbSet<POCollectionTrackerForAvi> POCollectionTrackersForAvi { get; set; }
        public DbSet<POBreakDownForAvi> POBreakDownsForAvi { get; set; }
        public DbSet<PrDetails> PrDetails { get; set; }
        public DbSet<PoIncoTerm> PoIncoTerms { get; set; }
        public DbSet<PoConfirmationDate> PoConfirmationDates { get; set; }
        public DbSet<PoConfirmationDateHistory> PoConfirmationDateHistories { get; set; }
        public DbSet<PoConfirmationHistory> PoConfirmationHistories { get; set; }
        public DbSet<PoItemHistory> PoItemHistories { get; set; }
        public DbSet<PRItemsDocumentUpload> PRItemsDocumentUploads { get; set; }
        public DbSet<PurchaseOrderSPReport> PurchaseOrderSPReports { get; set; }
        public DbSet<Tras_POSPReport> Tras_POSPReports { get; set; }
        public DbSet<Tras_PO_ConfirmationDate> Tras_POReport_ConfirmationDates { get; set; }
        public DbSet<poconfirmation_report_Dto> poconfirmation_Report_Dtos { get; set; }
        public DbSet<podeliveryschedule_report_Dto> podeliveryschedule_Report_Dtos { get; set; }
        //public DbSet<poproject_report_Dto> poproject_Report_Dtos { get; set; }
        public DbSet<PurchaseRequisitionSPReport> PurchaseRequisitionSPReports { get; set; }
        public DbSet<PurchaseOrderApprovalSPReport> PurchaseOrderApprovalSPReports { get; set; }
        public DbSet<PurchaseOrderSPReportForTrans> PurchaseOrderSPReportsForTrans { get; set; }
        public DbSet<PurchaseRequisitionSPReportForTrans> PurchaseRequisitionSPReportForTrans { get; set; }
        public DbSet<PurchaseOrderSPReportForAvision> PurchaseOrderSPReportForAvisions { get; set; }
        public DbSet<PoProjectSPReport> PoProjectSPReports { get; set; }
        public DbSet<PurchaseOrderDashboardSPReport> PurchaseOrderDashboardSPReports { get; set; }
        public DbSet<PaymentSPReport> PaymentSPReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrderSPReport>().HasNoKey();
            modelBuilder.Entity<Tras_POSPReport>().HasNoKey();
            modelBuilder.Entity<Tras_PO_ConfirmationDate>().HasNoKey();
            modelBuilder.Entity<poconfirmation_report_Dto>().HasNoKey();
            modelBuilder.Entity<podeliveryschedule_report_Dto>().HasNoKey();
            //modelBuilder.Entity<poproject_report_Dto>().HasNoKey();
            modelBuilder.Entity<PoProjectSPReport>().HasNoKey();
            modelBuilder.Entity<PurchaseRequisitionSPReport>().HasNoKey();
            modelBuilder.Entity<PurchaseOrderApprovalSPReport>().HasNoKey();
            modelBuilder.Entity<PurchaseOrderSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<PurchaseRequisitionSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<PurchaseOrderSPReportForAvision>().HasNoKey();
            modelBuilder.Entity<PurchaseRequisitionSPReportForAvision>().HasNoKey();
            modelBuilder.Entity<PurchaseOrderUnitListSPReportWithParamForTrans>().HasNoKey();
            modelBuilder.Entity<PayableSPReport>().HasNoKey();
            modelBuilder.Entity<PurchaseOrderDashboardSPReport>().HasNoKey();
            modelBuilder.Entity<podeliveryschedule_report_with_parameters_with_pagination_Dto>().HasNoKey();
            modelBuilder.Entity<poconfirmation_report_with_pagination_Dto>().HasNoKey();

            modelBuilder.Entity<PaymentSPReport>().HasNoKey();
        }
    }
}
