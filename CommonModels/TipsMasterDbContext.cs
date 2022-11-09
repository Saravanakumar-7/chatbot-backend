using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class TipsMasterDbContext : DbContext
    {
        public TipsMasterDbContext(DbContextOptions<TipsMasterDbContext> options) : base(options)   
        {

        }

        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<DeliveryTerm> DeliveryTerms { get; set; }  
        public DbSet<LeadTime> LeadTimes { get; set; }
        public DbSet<MaterialType>? MaterialTypes { get; set; }
        public DbSet<ItemMaster> ItemMasters { get; set; }

        public DbSet<Bank>? Banks { get; set; }
        public DbSet<IncoTerm>? IncoTerms { get; set; } 
        public DbSet<Currency>? Currencies { get; set; }
        public DbSet<Department>? Departments { get; set; }

        public DbSet<ScopeOfSupply>? ScopeOfSupplies { get; set; }
        public DbSet<VendorDepartment>? VendorDepartments { get; set; }
        public DbSet<BasicOfApproval>? BasicOfApprovals { get; set; }
        public DbSet<VendorCategory>? VendorCategories { get; set; }

        public DbSet<VendorContacts>? Contact { get; set; }
        public DbSet<VendorMaster>? VendorMasters { get; set; }
        public DbSet<VendorAddress>? Addresses { get; set; }
        public DbSet<VendorBanking>? VendorBankings { get; set; }

        public DbSet<VolumeUom>? VolumeUoms { get; set; }
        public DbSet<WeightUom>? WeightUoms { get; set; }
        
        public DbSet<ProcurementType>? ProcurementTypes { get; set; }

    }
}
