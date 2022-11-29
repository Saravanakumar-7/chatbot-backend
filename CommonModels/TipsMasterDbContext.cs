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
        public DbSet<LeadTime> LeadTimes { get; set; }
        public DbSet<MaterialType>? MaterialTypes { get; set; }
        public DbSet<ProcurementType>? ProcurementTypes { get; set; }

        public DbSet<ShipmentInstructions> ShipmentInstructions { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<RiskCategory> RiskCategory { get; set; }
        public DbSet<QuoteTerms> QuoteTerms { get; set; }
        public DbSet<Segment> Segment { get; set; }
        public DbSet<PreferredFreightForwarder> preferredFreightForwarders { get; set; }
        public DbSet<GST_Percentage> gst_Percentages { get; set; }
        public DbSet<PriceList> priceLists { get; set; }
        public DbSet<ShipmentMode> shipmentModes { get; set; }
        // pavan
        public DbSet<CostCenter> costCenters { get; set; }
        public DbSet<PurchaseGroup> purchaseGroups { get; set; }
        public DbSet<CostingMethod> CostingMethods { get; set; }
        public DbSet<AuditFrequency> auditFrequencies { get; set; }
        public DbSet<NatureOfRelationship> natureOfRelationships { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Salutations> salutations { get; set; }
        public DbSet<ExportUnitType> exportUnitTypes { get; set; }
        public DbSet<TypeOfCompany> typeOfCompanies { get; set; }
        public DbSet<PaymentTerm> paymentTerms { get; set; }
        public DbSet<PackingInstruction> packingInstructions { get; set; }

        public DbSet<Process> processes { get; set; }
        public DbSet<HeadCounting>? HeadCountings { get; set; }
        public DbSet<CompanyMasterHeadCounting>? CompanyMasterHeadCountings { get; set; }

        public DbSet<CustomerMasterHeadCounting>? CustomerMasterHeadCountings { get; set; }
        public DbSet<UOM>Uom { get; set; }
        public DbSet<UOC> Uoc { get; set; }
        public DbSet<Commodity> Commodity { get; set; }
        public DbSet<Locations> Locations { get; set; }

        public DbSet<CompanyMaster> CompanyMasters { get; set; }
        public DbSet<CompanyAddresses> CompanyAddresse { get; set; }
        public DbSet<CompanyContacts> CompanyContacts { get; set; }
        public DbSet<CompanyBanking> CompanyBankings { get; set; }

        public DbSet<ItemMaster> ItemMasters { get; set; }

        public DbSet<CustomerMaster> CustomerMasters { get; set; }
        public DbSet<CustomerContacts> CustomerContacts { get; set; }
        public DbSet<CustomerAddresses> CustomerAddresses{ get; set; }
        public DbSet<CustomerShippingAddresses> CustomerShippingAddresses{ get; set; }
        public DbSet<CustomerBanking> CustomerBankings{ get; set; }

        public DbSet<Bank>? Banks { get; set; }
        public DbSet<IncoTerm>? IncoTerms { get; set; } 
        public DbSet<Currency>? Currencies { get; set; }
        public DbSet<Department>? Departments { get; set; }

        public DbSet<ScopeOfSupply>? ScopeOfSupplies { get; set; }
        public DbSet<VendorDepartment>? VendorDepartments { get; set; }
        public DbSet<BasisOfApproval>? BasisOfApprovals { get; set; }
        public DbSet<VendorCategory>? VendorCategories { get; set; }

        public DbSet<VendorContacts>? Contact { get; set; }
        public DbSet<VendorMaster>? VendorMasters { get; set; }
        public DbSet<VendorAddress>? Addresses { get; set; }
        public DbSet<VendorBanking>? VendorBankings { get; set; }


        public DbSet<VolumeUom>? VolumeUoms { get; set; }
        public DbSet<WeightUom>? WeightUoms { get; set; }
        public DbSet<DeliveryTerm> DeliveryTerms { get; set; }


        public DbSet<NREConsumable> BomNREConsumables { get; set; }
        public DbSet<EnggBom> EnggBoms { get; set; }
        public DbSet<EnggChildItem> EnggChildItems { get; set; }
        public DbSet<EnggAlternates> EnggAlternates { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Inventory_Transaction> Inventory_Transactions { get; set; }

        public DbSet<PartTypes> PartTypes { get; set; }



    }
}
