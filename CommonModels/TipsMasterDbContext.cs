using Entities;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.InteropServices;

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
        public DbSet<OrderType>? OrderTypes { get; set; }
        public DbSet<ShipmentInstructions> ShipmentInstructions { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<RiskCategory> RiskCategory { get; set; }
        public DbSet<QuoteTerms> QuoteTerms { get; set; }
        public DbSet<Segment> Segment { get; set; }
        public DbSet<PreferredFreightForwarder> PreferredFreightForwarders { get; set; }
        public DbSet<GST_Percentage> Gst_Percentages { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<ShipmentMode> ShipmentModes { get; set; }
        // pavan
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<PurchaseGroup> PurchaseGroups { get; set; }
        public DbSet<CostingMethod> CostingMethods { get; set; }
        public DbSet<AuditFrequency> AuditFrequencies { get; set; }
        public DbSet<NatureOfRelationship> NatureOfRelationships { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Salutations> Salutations { get; set; }
        public DbSet<ExportUnitType> ExportUnitTypes { get; set; }
        public DbSet<TypeOfCompany> TypeOfCompanies { get; set; }
        public DbSet<PaymentTerm> PaymentTerms { get; set; }
        public DbSet<PackingInstruction> PackingInstructions { get; set; }

        public DbSet<Process> Processes { get; set; }
        public DbSet<VendorHeadCounting>? HeadCountings { get; set; }
        public DbSet<CompanyMasterHeadCounting>? CompanyMasterHeadCountings { get; set; }
        public DbSet<CompanyOtherUploads>? CompanyOtherUploads { get; set; }
        public DbSet<CustomerMasterHeadCounting>? CustomerMasterHeadCountings { get; set; }
        public DbSet<CustomerOtherUploads>? CustomerOtherUploads { get; set; }
        public DbSet<VendorOtherUploads>? VendorOtherUploads { get; set; }
        public DbSet<UOM> Uom { get; set; }
        public DbSet<UOC> Uoc { get; set; }
        public DbSet<Commodity> Commodity { get; set; }
        public DbSet<Locations> Locations { get; set; }

        public DbSet<CompanyMaster> CompanyMasters { get; set; }
        public DbSet<CompanyAddresses> CompanyAddresse { get; set; }
        public DbSet<CompanyContacts> CompanyContacts { get; set; }
        public DbSet<CompanyBanking> CompanyBankings { get; set; }

        public DbSet<ItemMaster> ItemMasters { get; set; }
        public DbSet<ItemmasterAlternate> ItemmasterAlternates { get; set; }
        public DbSet<ItemMasterRouting> ItemMasterRoutings { get; set; }


        public DbSet<CustomerMaster> CustomerMasters { get; set; }
        public DbSet<CustomerContacts> CustomerContacts { get; set; }
        public DbSet<CustomerAddresses> CustomerAddresses { get; set; }
        public DbSet<CustomerShippingAddresses> CustomerShippingAddresses { get; set; }
        public DbSet<CustomerBanking> CustomerBankings { get; set; }

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

        public DbSet<Inventory1> Inventories { get; set; }

        public DbSet<Inventory_Transaction> Inventory_Transactions { get; set; }

        public DbSet<PartTypes> PartTypes { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadAddress> LeadAddresses { get; set; }

        public DbSet<DemoStatus> DemoStatuses { get; set; }
        public DbSet<LeadStatus> LeadStatuses { get; set; }
        public DbSet<LeadType> LeadTypes { get; set; }
        public DbSet<SecondarySource> SecondarySources { get; set; }
        public DbSet<Source> Sources { get; set; }

        public DbSet<EngineeringBom> EngineeringBoms { get; set; }
        public DbSet<CostingBom> CostingBoms { get; set; }
        public DbSet<ProductionBom> ProductionBoms { get; set; }

        public DbSet<EnggBomGroup> BomGroups { get; set; }
        public DbSet<EnggCustomField> CustomFields { get; set; }
        public DbSet<ProductType>? ProductTypes { get; set; }
        public DbSet<RoomNames>? RoomNames { get; set; }
        public DbSet<TypeSolution> TypeSolutions { get; set; }

        public DbSet<FileUpload>? fileUploads { get; set; }

        public DbSet<ImageUpload>? imageUploads { get; set; }

        public DbSet<BHK>? BHKs { get; set; }
        public DbSet<StageOfConstruction>? StageOfConstructions { get; set; }
        public DbSet<SourceDetails>? sourceDetails { get; set; }
        public DbSet<State>? states { get; set; }
        public DbSet<Architectures>? Architectures { get; set; }
        public DbSet<PmcContractor>? PmcContractors { get; set; }
        public DbSet<LightningDesigner>? LightningDesigners { get; set; }
        public DbSet<City>? Cities { get; set; }

        public DbSet<TypeOfHome> typeOfHomes { get; set; }

        public DbSet<SFT> SFTs { get; set; }

        public DbSet<ProjectName>? ProjectNames { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleAccess> RoleAccesses { get; set; }
        public DbSet<RegistrationForm> RegistrationForms { get; set; }
        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<FormsAccess> FormsAccesses { get; set; }
        public DbSet<LeadWebsite> LeadWebsites { get; set; }
        public DbSet<Unit> Units { get; set; }

        public DbSet<IssuingStock> IssuingStocks { get; set; }
        public DbSet<VendorRelatedVendor>? RelatedVendors { get; set; }
        public DbSet<CustomerRelatedCustomer> RelatedCustomers { get; set; }

        public DbSet<AdditionalCharges> AdditionalCharges { get; set; }
        public DbSet<CustomerCategory> CustomerCategories { get; set; }
        public DbSet<CompanyCategory> CompanyCategories { get; set; }
        public DbSet<OtherCharges> OtherCharges { get; set; }
        public DbSet<CompanyApproval> CompanyApprovals { get; set; }
        public DbSet<CompanyFileUpload> CompanyFileUploads { get; set; }
        public DbSet<NoOfRoom> NoOfRooms { get; set; }
        public DbSet<TypeOfRoom> TypeOfRooms { get; set; }
        public DbSet<VendorId> VendorIds { get; set; }
        public DbSet<CSNOs>? CSNOs { get; set; }
        public DbSet<Convertionrate> Convertionrates { get; set; }
        public DbSet<EnggBomSPReport> EnggBomSPReports { get; set; }
        public DbSet<WeightedAvgRate> weighted_avg_rate { get; set; }
        public DbSet<EmailTemplate> emailtemplate { get; set; }
        public DbSet<EmailIDs> EmailIDs { get; set; }
        public DbSet<CustomerMasterLeadIdSPReport> CustomerMasterLeadIdSPReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnggBomSPReport>().HasNoKey();
            modelBuilder.Entity<CustomerMasterLeadIdSPReport>().HasNoKey();
            modelBuilder.Entity<FGCostingSPReport>().HasNoKey();
        }


    }
}