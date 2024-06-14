using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Entities
{
    public class TipsProductionDbContext : DbContext
    {

        public TipsProductionDbContext(DbContextOptions<TipsProductionDbContext> options) : base(options)
        {

        }
        public DbSet<OQCBinningLocation> OQCBinningLocation { get; set; }
        public DbSet<OQCBinning> OQCBinning { get; set; }
        public DbSet<PickList> PickLists { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<ShopOrderItem> ShopOrderItems { get; set; }
        public DbSet<ShopOrderConfirmation> ShopOrderConfirmations { get; set; }
        public DbSet<SAShopOrder> SAShopOrders { get; set; }
        public DbSet<SAShopOrderItem> SAShopOrderItems { get; set; }
        public DbSet<SAShopOrderMaterialIssue> SAShopOrderMaterialIssues { get; set; }
        public DbSet<SAShopOrderMaterialIssueGeneral> SAShopOrderMaterialIssueGenerals { get; set; }
        public DbSet<FGShopOrderMaterialIssue> FGShopOrderMaterialIssues { get; set; }
        public DbSet<FGShopOrderMaterialIssueGeneral> FGShopOrderMaterialIssueGenerals { get; set; }
        public DbSet<MaterialReturnNote> MaterialReturnNotes { get; set; }

        public DbSet<MRNWarehouseDetails> MRNWarehouses { get; set; }
        public DbSet<MRNNumber> MRNNumbers { get; set; }
        public DbSet<MaterialReturnNoteItem> MaterialReturnNoteItems { get; set; }
        public DbSet<MaterialIssue> MaterialIssue { get; set; }

        public DbSet<MaterialIssueItem> MaterialIssueItems { get; set; }
        public DbSet<OQC> oQCs { get; set; }
        public DbSet<MRNumber> MRNumbers { get; set; }
        public DbSet<MaterialRequests> MaterialRequests { get; set; }
        public DbSet<MRStockDetails> MRStockDetails { get; set; }
        public DbSet<MaterialRequestItems> MaterialRequestItems { get; set; }
        public DbSet<MaterialIssueHistory> MaterialIssueHistories { get; set; }
        public DbSet<SONumber> SONumbers { get; set; }
        public DbSet<ShopOrderNumberSPReport> ShopOrderNumberSPReports { get; set; }
        public DbSet<MaterialIssueSPReport> MaterialIssueSPReports { get; set; }
        public DbSet<MaterialRequestSPReport> MaterialRequestSPReports { get; set; }
        public DbSet<OQCSPReport> OQCSPReports { get; set; }
        public DbSet<OQCBinningSPReport> OQCBinningSPReports { get; set; }
        public DbSet<OQCAndOQCBinningSPReport> OQCAndOQCBinningSPReports { get; set; }
        public DbSet<PickListDTO> PickListDTOs { get; set; }
        public DbSet<MaterialIssueAgainstMRSPReport> MaterialIssueAgainstMRSPReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopOrderNumberSPReport>().HasNoKey();
            modelBuilder.Entity<MaterialIssueSPReport>().HasNoKey();
            modelBuilder.Entity<MaterialRequestSPReport>().HasNoKey();
            modelBuilder.Entity<OQCSPReport>().HasNoKey();
            modelBuilder.Entity<OQCBinningSPReport>().HasNoKey();
            modelBuilder.Entity<PickList>().HasNoKey();
            modelBuilder.Entity<PickListDTO>().HasNoKey();
            modelBuilder.Entity<OQCAndOQCBinningSPReport>().HasNoKey();
            modelBuilder.Entity<MaterialIssueAgainstMRSPReport>().HasNoKey();
            // Other entity configurations can be added here

            base.OnModelCreating(modelBuilder);
        }
    }
}
