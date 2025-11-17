using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using Tips.Grin.Api.Controllers;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Entities
{
    public class TipsGrinDbContext : DbContext
    {

        public TipsGrinDbContext(DbContextOptions<TipsGrinDbContext> options) : base(options)
        {

        }
        public DbSet<IQCReturnToVendor> IQCReturnToVendor { get; set; }
        public DbSet<IQCReturnToVendorItems> IQCReturnToVendorItems { get; set; }
        public DbSet<IQCForServiceItems_Items> IQCForServiceItems_Items { get; set; }
        public DbSet<IQCForServiceItems> IQCForServiceItems { get; set; }
        public DbSet<GrinsForServiceItemsNumber> GrinsForServiceItemsNumber { get; set; }
        public DbSet<GrinsForServiceItems> GrinsForServiceItems { get; set; }
        public DbSet<GrinsForServiceItemsParts> GrinsForServiceItemsParts { get; set; }
        public DbSet<GrinsForServiceItemsOtherCharges> GrinsForServiceItemsOtherCharges { get; set; }
        public DbSet<GrinsForServiceItemsProjectNumbers> GrinsForServiceItemsProjectNumbers { get; set; }
        public DbSet<Grins> Grins { get; set; }
        public DbSet<WeightedAvgCost> WeightedAvgCosts { get; set; }
        public DbSet<GrinParts> GrinParts { get; set; }
        public DbSet<GrinNumber> GrinNumbers { get; set; }
        public DbSet<KIT_GrinNumber> KIT_GrinNumbers { get; set; }
        public DbSet<IQCConfirmation> IQCConfirmations { get; set; }
        public DbSet<IQCConfirmationItems> IQCConfirmationItems { get; set; }
        public DbSet<Binning> Binnings { get; set; }
        public DbSet<BinningItems> BinningItem { get; set; }

        public DbSet<BinningLocation> BinningLocations { get; set; }
        public DbSet<ProjectNumbers> ProjectNumbers { get; set; }
        public DbSet<DocumentUpload> DocumentUploads { get; set; }
        public DbSet<ReturnGrin> ReturnGrins { get; set; }
        public DbSet<ReturnGrinParts> ReturnGrinParts { get; set; }
        public DbSet<ReturnGrinDocumentUpload> ReturnGrinDocumentUploads { get; set; }
        public DbSet<OpenGrin> OpenGrin { get; set; }
        public DbSet<OpenGrinParts> OpenGrinParts { get; set; }
        public DbSet<OtherCharges> OtherCharges { get; set; }
        public DbSet<OpenGrinNumber> OpenGrinNumbers { get; set; }

        public DbSet<OpenGrinDetails> OpenGrinDetails { get; set; }
        public DbSet<Grin_ReportSP> Grin_ReportSPs { get; set; }
        public DbSet<BinningPendingReportSp> BinningPendingReportSp { get; set; }
        public DbSet<OpenGrin_SPReport> OpenGrin_SPReports { get; set; }
        public DbSet<IQCConfirmation_SPReport> IQCConfirmation_SPReports { get; set; }
        public DbSet<GrinSPReportForTrans> GrinSPReportForTrans { get; set; }
        public DbSet<OpenGrinSpReportForTrans> OpenGrinSpReports { get; set; }
        public DbSet<OpenGrinSpReportForTrs> OpenGrinSpReportForTrs { get; set; }
        public DbSet<IQCConfirmationSPReportForTrans> IQCConfirmationSPReportForTrans { get; set; }
        public DbSet<OpenGrinForGrin> OpenGrinForGrins { get; set; }
        public DbSet<OpenGrinForGrinItems> OpenGrinForGrinItems { get; set; }
        public DbSet<OpenGrinForGrinProjectNumber> OpenGrinForGrinProjectNumbers { get; set; }
        public DbSet<OpenGrinForGrinNumber> OpenGrinForGrinNumbers { get; set; }
        public DbSet<OpenGrinForIQC> OpenGrinForIQCs { get; set; }
        public DbSet<OpenGrinForIQCItems> OpenGrinForIQCItems { get; set; }
        public DbSet<OpenGrinForBinning> OpenGrinForBinnings { get; set; }
        public DbSet<OpenGrinForBinningItems> OpenGrinForBinningItems { get; set; }
        public DbSet<OpenGrinForBinningLocations> OpenGrinForBinningLocations { get; set; }
        public DbSet<IQCPendingReportWithParamForTrans> IQCPendingReportWithParamForTrans { get; set; }
        public DbSet<KIT_GRIN> KIT_GRIN { get; set; }
        public DbSet<KIT_GRINParts> KIT_GRINParts { get; set; }
        public DbSet<KIT_GRIN_OtherCharges> KIT_GRIN_OtherCharges { get; set; }
        public DbSet<KIT_GRIN_ProjectNumbers> KIT_GRIN_ProjectNumbers { get; set; }
        public DbSet<KIT_GRIN_KITComponents> KIT_GRIN_KITComponents { get; set; }
        public DbSet<KIT_IQC> KIT_IQC { get; set; }
        public DbSet<KIT_IQCItems> KIT_IQCItems { get; set; }
        public DbSet<KIT_Binning> KIT_Binning { get; set; }
        public DbSet<KIT_BinningItems> KIT_BinningItems { get; set; }
        public DbSet<KIT_BinningItemsLocation> KIT_BinningItemsLocation { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grin_ReportSP>().HasNoKey();
            modelBuilder.Entity<BinningPendingReportSp>().HasNoKey();
            modelBuilder.Entity<IQCConfirmation_SPReport>().HasNoKey();
            modelBuilder.Entity<OpenGrin_SPReport>().HasNoKey();
            modelBuilder.Entity<GrinSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<OpenGrinSpReportForTrans>().HasNoKey();
            modelBuilder.Entity<IQCConfirmationSPReportForTrans>().HasNoKey();
            modelBuilder.Entity<GrinForServiceItemsSPReport>().HasNoKey();
            modelBuilder.Entity<KITGrinSPReport>().HasNoKey();
            modelBuilder.Entity<IQCForServiceItemsSPReport>().HasNoKey();
            modelBuilder.Entity<OpenGrinForGrinSPReport>().HasNoKey();
            modelBuilder.Entity<OpenGrinForIQCSPReport>().HasNoKey();
            modelBuilder.Entity<IQCPendingReportWithParamForTrans>().HasNoKey();
            modelBuilder.Entity<PurchaseInventorySPReport>().HasNoKey();
            modelBuilder.Entity<IQCConfirmationSPReportForAvi>().HasNoKey();
            modelBuilder.Entity<GrinSPReportForAvi>().HasNoKey();
            modelBuilder.Entity<BinningSPReportAvi>().HasNoKey();
            modelBuilder.Entity<PoAndGrinUnitPriceSPReport>().HasNoKey();
            modelBuilder.Entity<OpenGrinSpReportForTrs>().HasNoKey();
            
        }
    }
}
