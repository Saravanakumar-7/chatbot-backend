using Microsoft.EntityFrameworkCore;
using System;
using Tips.Grin.Api.Controllers;

namespace Tips.Grin.Api.Entities
{
    public class TipsGrinDbContext : DbContext
    {

        public TipsGrinDbContext(DbContextOptions<TipsGrinDbContext> options) : base(options)
        {

        }
        public DbSet<Grins> Grins { get; set; }
        public DbSet<WeightedAvgRate> weighted_avg_rate { get; set; }
        public DbSet<WeightedAvgCost> WeightedAvgCosts { get; set; }
        public DbSet<GrinParts> GrinParts { get; set; }
        public DbSet<GrinNumber> GrinNumbers { get; set; }
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
        public DbSet<OpenGrin_SPReport> OpenGrin_SPReports { get; set; }
        public DbSet<IQCConfirmation_SPReport> IQCConfirmation_SPReports { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grin_ReportSP>().HasNoKey();
            modelBuilder.Entity<IQCConfirmation_SPReport>().HasNoKey();
            modelBuilder.Entity<OpenGrin_SPReport>().HasNoKey();
        }


    }
}
