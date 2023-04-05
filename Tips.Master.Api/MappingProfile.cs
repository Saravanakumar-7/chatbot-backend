using AutoMapper;
using Entities;
using Entities.DTOs;
using System.Runtime.InteropServices;

namespace Tips.Master.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerType , CustomerTypeDto>().ReverseMap();
            CreateMap<CustomerType, CustomerTypeDtoPost>().ReverseMap();
            CreateMap<CustomerType, CustomerTypeDtoUpdate>().ReverseMap();

            CreateMap<ShipmentInstructions, ShipmentInstructionsDto>().ReverseMap();
            CreateMap<ShipmentInstructions, ShipmentInstructionsDtoPost>().ReverseMap();
            CreateMap<ShipmentInstructions, ShipmentInstructionsDtoUpdate>().ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDtoPost>().ReverseMap();
            CreateMap<Category, CategoryDtoUpdate>().ReverseMap();

            CreateMap<RiskCategory, RiskCategoryDto>().ReverseMap();
            CreateMap<RiskCategory, RiskCategoryDtoPost>().ReverseMap();
            CreateMap<RiskCategory, RiskCategoryDtoUpdate>().ReverseMap();

            CreateMap<QuoteTerms, QuoteTermsDto>().ReverseMap();
            CreateMap<QuoteTerms, QuoteTermsDtoPost>().ReverseMap();
            CreateMap<QuoteTerms, QuoteTermsDtoUpdate>().ReverseMap();

            CreateMap<Segment, SegmentDto>().ReverseMap();
            CreateMap<Segment, SegmentDtoPost>().ReverseMap();
            CreateMap<Segment, SegmentDtoUpdate>().ReverseMap();

            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<Warehouse, WarehouseDtoPost>().ReverseMap();
            CreateMap<Warehouse, WarehouseDtoUpdate>().ReverseMap();

            CreateMap<UOM, UOMDto>().ReverseMap();
            CreateMap<UOM, UOMDtoPost>().ReverseMap();
            CreateMap<UOM, UOMDtoUpdate>().ReverseMap();

            CreateMap<UOC, UOCDto>().ReverseMap();
            CreateMap<UOC, UOCDtoPost>().ReverseMap();
            CreateMap<UOC, UOCDtoUpdate>().ReverseMap();

            CreateMap<Commodity, CommodityDto>().ReverseMap();
            CreateMap<Commodity, CommodityDtoPost>().ReverseMap();
            CreateMap<Commodity, CommodityDtoUpdate>().ReverseMap();

            CreateMap<Locations, LocationsDto>().ReverseMap();
            CreateMap<Locations, LocationsDtoPost>().ReverseMap();
            CreateMap<Locations, LocationsDtoUpdate>().ReverseMap();
            CreateMap<Locations, GetListofLocationsByWarehouseDto>().ReverseMap();


            CreateMap<CustomerMaster, CustomerMasterDto>().ReverseMap();
            CreateMap<CustomerMaster, CustomerMasterDtoPost>().ReverseMap();
            CreateMap<CustomerMaster, CustomerMasterDtoUpdate>().ReverseMap();

            CreateMap<CustomerShippingAddresses, CustomerShippingAddressesDto>().ReverseMap();
            CreateMap<CustomerShippingAddresses, CustomerShippingAddressesDtoPost>().ReverseMap();
            CreateMap<CustomerShippingAddresses, CustomerShippingAddressesDtoUpdate>().ReverseMap();

            CreateMap<CustomerAddresses, CustomerAddressesDto>().ReverseMap();
            CreateMap<CustomerAddresses, CustomerAddressesDtoPost>().ReverseMap();
            CreateMap<CustomerAddresses, CustomerAddressesDtoUpdate>().ReverseMap();

            CreateMap<CustomerContacts, CustomerContactsDto>().ReverseMap();
            CreateMap<CustomerContacts, CustomerContactsDtoPost>().ReverseMap();
            CreateMap<CustomerContacts, CustomerContactsDtoUpdate>().ReverseMap();

            CreateMap<CustomerBanking, CustomerBankingDto>().ReverseMap();
            CreateMap<CustomerBanking, CustomerBankingDtoPost>().ReverseMap();
            CreateMap<CustomerBanking, CustomerBankingDtoUpdate>().ReverseMap();

            CreateMap<CompanyMaster, CompanyMasterDto>().ReverseMap();
            CreateMap<CompanyMaster, CompanyMasterDtoPost>().ReverseMap();
            CreateMap<CompanyMaster, CompanyMasterDtoUpdate>().ReverseMap();       

            CreateMap<CompanyAddresses, CompanyAddressesDto>().ReverseMap();
            CreateMap<CompanyAddresses, CompanyAddressesDtoPost>().ReverseMap();
            CreateMap<CompanyAddresses, CompanyAddressesDtoUpdate>().ReverseMap();

            CreateMap<CompanyContacts, CompanyContactsDto>().ReverseMap();
            CreateMap<CompanyContacts, CompanyContactsDtoPost>().ReverseMap();
            CreateMap<CompanyContacts, CompanyContactsDtoUpdate>().ReverseMap();

            CreateMap<CompanyBanking, CompanyBankingDto>().ReverseMap();
            CreateMap<CompanyBanking, CompanyBankingDtoPost>().ReverseMap();
            CreateMap<CompanyBanking, CompanyBankingDtoUpdate>().ReverseMap();

            CreateMap<PurchaseGroup, PurchaseGroupDto>().ReverseMap();
            CreateMap<PurchaseGroup, PurchaseGroupDtoPost>().ReverseMap();
            CreateMap<PurchaseGroup, PurchaseGroupDtoUpdate>().ReverseMap();


            CreateMap<CostCenter, CostCenterDto>().ReverseMap();
            CreateMap<CostCenter, CostCenterDtoPost>().ReverseMap();
            CreateMap<CostCenter, CostCenterDtoUpdate>().ReverseMap();

            CreateMap<CostingMethod, CostingMethodDto>().ReverseMap();
            CreateMap<CostingMethod, CostingMethodDtoPost>().ReverseMap();
            CreateMap<CostingMethod, CostingMethodDtoUpdate>().ReverseMap();

            CreateMap<AuditFrequency, AuditFrequencyDto>().ReverseMap();
            CreateMap<AuditFrequency, AuditFrequencyDtoPost>().ReverseMap();
            CreateMap<AuditFrequency, AuditFrequencyDtoUpdate>().ReverseMap();

            CreateMap<NatureOfRelationship, NatureOfRelationshipDto>().ReverseMap();
            CreateMap<NatureOfRelationship, NatureOfRelationshipDtoPost>().ReverseMap();
            CreateMap<NatureOfRelationship, NatureOfRelationshipDtoUpdate>().ReverseMap();

            CreateMap<Language, LanguageDto>().ReverseMap();
            CreateMap<Language, LanguageDtoPost>().ReverseMap();
            CreateMap<Language, LanguageDtoUpdate>().ReverseMap();

            CreateMap<Salutations, SalutationsDto>().ReverseMap();
            CreateMap<Salutations, SalutationsDtoPost>().ReverseMap();
            CreateMap<Salutations, SalutationsDtoUpdate>().ReverseMap();

            CreateMap<ExportUnitType, ExportUnitTypeDto>().ReverseMap();
            CreateMap<ExportUnitType, ExportUnitTypeDtoPost>().ReverseMap();
            CreateMap<ExportUnitType, ExportUnitTypeDtoUpdate>().ReverseMap();


            CreateMap<TypeOfCompany, TypeOfCompanyDto>().ReverseMap();
            CreateMap<TypeOfCompany, TypeOfCompanyDtoPost>().ReverseMap();
            CreateMap<TypeOfCompany, TypeOfCompanyDtoUpdate>().ReverseMap();

            CreateMap<PaymentTerm, PaymentTermDto>().ReverseMap();
            CreateMap<PaymentTerm, PaymentTermDtoPost>().ReverseMap();
            CreateMap<PaymentTerm, PaymentTermDtoUpdate>().ReverseMap();

            CreateMap<LeadWebsite, LeadWebsiteDto>().ReverseMap();
            CreateMap<LeadWebsite, LeadWebsitePostDto>().ReverseMap();
            CreateMap<LeadWebsite, LeadWebsiteUpdateDto>().ReverseMap();


            CreateMap<PackingInstruction, PackingInstructionDto>().ReverseMap();
            CreateMap<PackingInstruction, PackingInstructionDtoPost>().ReverseMap();
            CreateMap<PackingInstruction, PackingInstructionDtoUpdate>().ReverseMap();

            CreateMap<LeadTime, LeadTimeDto>().ReverseMap();
            CreateMap<LeadTime, LeadTimeDtoPost>().ReverseMap();
            CreateMap<LeadTime, LeadTimeDtoUpdate>().ReverseMap();

            CreateMap<MaterialType, MaterialTypeDto>().ReverseMap();
            CreateMap<MaterialType, MaterialTypeDtoPost>().ReverseMap();
            CreateMap<MaterialType, MaterialTypeDtoUpdate>().ReverseMap();

            CreateMap<ProcurementType, ProcurementTypeDto>().ReverseMap();
            CreateMap<ProcurementType, ProcurementTypeDtoPost>().ReverseMap();
            CreateMap<ProcurementType, ProcurementTypeDtoUpdate>().ReverseMap();

            CreateMap<ItemMaster, ItemMasterDto>().ReverseMap();
            CreateMap<ItemMaster, ItemMasterDtoPost>().ReverseMap();
            CreateMap<ItemMaster, ItemMasterDtoUpdate>().ReverseMap();

            CreateMap<ItemmasterAlternate, ItemmasterAlternateDto>().ReverseMap();
            CreateMap<ItemmasterAlternate, ItemmasterAlternateDtoPost>().ReverseMap();
            CreateMap<ItemmasterAlternate, ItemmasterAlternateDtoUpdate>().ReverseMap();

            CreateMap<ItemMasterApprovedVendor, ItemMasterApprovedVendorDto>().ReverseMap();
            CreateMap<ItemMasterApprovedVendor, ItemMasterApprovedVendorDtoPost>().ReverseMap();
            CreateMap<ItemMasterApprovedVendor, ItemMasterApprovedVendorDtoUpdate>().ReverseMap();

            CreateMap<ItemMasterFileUpload, ItemMasterFileUploadDto>().ReverseMap();
            CreateMap<ItemMasterFileUpload, ItemMasterFileUploadDtoPost>().ReverseMap();
            CreateMap<ItemMasterFileUpload, ItemMasterFileUploadDtoUpdate>().ReverseMap();

            CreateMap<ItemMasterRouting, ItemMasterRoutingDto>().ReverseMap();
            CreateMap<ItemMasterRouting, ItemMasterRoutingDtoPost>().ReverseMap();
            CreateMap<ItemMasterRouting, ItemMasterRoutingDtoUpdate>().ReverseMap();

            CreateMap<ItemMasterWarehouse, ItemMasterWarehouseDto>().ReverseMap();
            CreateMap<ItemMasterWarehouse, ItemMasterWarehouseDtoPost>().ReverseMap();
            CreateMap<ItemMasterWarehouse, ItemMasterWarehouseDtoUpdate>().ReverseMap();

            CreateMap<Unit, UnitDto>().ReverseMap();
            CreateMap<Unit, UnitPostDto>().ReverseMap();
            CreateMap<Unit, UnitUpdateDto>().ReverseMap();



            CreateMap<DeliveryTerm, DeliveryTermGetDto>().ReverseMap();
            CreateMap<DeliveryTerm, DeliveryTermPostDto>().ReverseMap();
            CreateMap<DeliveryTerm, DeliveryTermUpdateDto>().ReverseMap();

            CreateMap<VolumeUom, VolumeUomDto>().ReverseMap();
            CreateMap<VolumeUom, VolumeUomPostDto>().ReverseMap();
            CreateMap<VolumeUom, VolumeUomUpdateDto>().ReverseMap();

            CreateMap<WeightUom, WeightUomDto>().ReverseMap();
            CreateMap<WeightUom, WeightUomPostDto>().ReverseMap();
            CreateMap<WeightUom, WeightUomUpdateDto>().ReverseMap();
         
            CreateMap<Bank, BankDto>().ReverseMap();
            CreateMap<Bank, BankPostDto>().ReverseMap();
            CreateMap<Bank, BankUpdateDto>().ReverseMap();

            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, DepartmentPostDto>().ReverseMap();
            CreateMap<Department, DepartmentUpdateDto>().ReverseMap();

            CreateMap<IncoTerm, IncoTermDto>().ReverseMap();
            CreateMap<IncoTerm, IncoTermPostDto>().ReverseMap();
            CreateMap<IncoTerm, IncoTermUpdateDto>().ReverseMap();

            CreateMap<Currency, CurrencyDto>().ReverseMap();
            CreateMap<Currency, CurrencyPostDto>().ReverseMap();
            CreateMap<Currency, CurrencyUpdateDto>().ReverseMap();
            
            CreateMap<ScopeOfSupply, ScopeOfSupplyDto>().ReverseMap();
            CreateMap<ScopeOfSupply, ScopeOfSupplyPostDto>().ReverseMap();
            CreateMap<ScopeOfSupply, ScopeOfSupplyUpdateDto>().ReverseMap();

            CreateMap<VendorCategory, VendorCategoryDto>().ReverseMap();
            CreateMap<VendorCategory, VendorCategoryPostDto>().ReverseMap();
            CreateMap<VendorCategory, VendorCategoryUpdateDto>().ReverseMap();

            CreateMap<VendorDepartment, VendorDepartmentDto>().ReverseMap();
            CreateMap<VendorDepartment, VendorDepartmentPostDto>().ReverseMap();
            CreateMap<VendorDepartment, VendorDepartmentUpdateDto>().ReverseMap();

            CreateMap<BasisOfApproval, BasisOfApprovalDto>().ReverseMap();
            CreateMap<BasisOfApproval, BasisOfApprovalPostDto>().ReverseMap();
            CreateMap<BasisOfApproval, BasisOfApprovalUpdateDto>().ReverseMap();
            //test
            CreateMap<VendorContacts, VendorContactsDto>().ReverseMap();
            CreateMap<VendorContacts, VendorContactsPostDto>().ReverseMap();
            CreateMap<VendorContacts, VendorContactsPostDto>().ReverseMap();

            CreateMap<VendorBanking, VendorBankingDto>().ReverseMap();
            CreateMap<VendorBanking, VendorBankingPostDto>().ReverseMap();
            CreateMap<VendorBanking, VendorBankingUpdateDto>().ReverseMap();

            CreateMap<VendorAddress, VendorAddressDto>().ReverseMap();
            CreateMap<VendorAddress, VendorAddressPostDto>().ReverseMap();
            CreateMap<VendorAddress, VendorAddressPostDto>().ReverseMap();

            CreateMap<VendorMaster, VendorMasterDto>().ReverseMap();
            CreateMap<VendorMaster, VendorMasterPostDto>().ReverseMap();
            CreateMap<VendorMaster, VendorMasterPostDto>().ReverseMap();

            CreateMap<VendorHeadCounting, VendorHeadCountingDto>().ReverseMap();
            CreateMap<VendorHeadCounting, HeadCountingPostDto>().ReverseMap();
            CreateMap<VendorHeadCounting, HeadCountingUpdateDto>().ReverseMap();

            CreateMap<PreferredFreightForwarder, PreferredFreightForwarderDto>().ReverseMap();
            CreateMap<PreferredFreightForwarder, PreferredFreightForwarderDtoPost>().ReverseMap();
            CreateMap<PreferredFreightForwarder, PreferredFreightForwarderDtoUpdate>().ReverseMap();


            CreateMap<GST_Percentage, GST_PercentageDto>().ReverseMap();
            CreateMap<GST_Percentage, GST_PercentageDtoPost>().ReverseMap();
            CreateMap<GST_Percentage, GST_PercentageDtoUpdate>().ReverseMap();

            CreateMap<PriceList, PriceListDto>().ReverseMap();
            CreateMap<PriceList, PriceListDtoPost>().ReverseMap();
            CreateMap<PriceList, PriceListDtoUpdate>().ReverseMap();

            CreateMap<ShipmentMode, ShipmentModeDto>().ReverseMap();
            CreateMap<ShipmentMode, ShipmentModeDtoPost>().ReverseMap();
            CreateMap<ShipmentMode, ShipmentModeDtoUpdate>().ReverseMap();

            CreateMap<NREConsumable, BomNREConsumableDto>().ReverseMap();
            CreateMap<NREConsumable, BomNREConsumablePostDto>().ReverseMap();
            CreateMap<NREConsumable, BomNREConsumableUpdateDto>().ReverseMap();

            CreateMap<EnggBom, EnggBomDto>().ReverseMap();
            CreateMap<EnggBom, EnggBomPostDto>().ReverseMap();

            CreateMap<EnggBom, EnggBomUpdateDto>().ReverseMap();



            CreateMap<EnggChildItem, EnggChildItemDto>().ReverseMap();
            CreateMap<EnggChildItem, EnggChildItemPostDto>().ReverseMap();
            CreateMap<EnggChildItem, EnggChildItemUpdateDto>().ReverseMap();

            CreateMap<EnggAlternates, EnggAlternatesDto>().ReverseMap();
            CreateMap<EnggAlternates, EnggAlternatesPostDto>().ReverseMap();
            CreateMap<EnggAlternates, EnggAlternatesUpdateDto>().ReverseMap();

            CreateMap<CompanyMasterHeadCounting, CompanyMasterHeadCountingDto>().ReverseMap();
            CreateMap<CompanyMasterHeadCounting, CompanyMasterHeadCountingDtoPost>().ReverseMap();
            CreateMap<CompanyMasterHeadCounting, CompanyMasterHeadCountingDtoUpdate>().ReverseMap();

            CreateMap<CustomerMasterHeadCounting, CustomerMasterHeadCountingDto>().ReverseMap();
            CreateMap<CustomerMasterHeadCounting, CustomerMasterHeadCountingDtoPost>().ReverseMap();
            CreateMap<CustomerMasterHeadCounting, CustomerMasterHeadCountingDtoUpdate>().ReverseMap();

            CreateMap<Process, ProcessDto>().ReverseMap();
            CreateMap<Process, ProcessDtoPost>().ReverseMap();
            CreateMap<Process, ProcessDtoUpdate>().ReverseMap();

            CreateMap<PartTypes, PartTypesDto>().ReverseMap();
            CreateMap<PartTypes, PartTypesDtoPost>().ReverseMap();
            CreateMap<PartTypes, PartTypesDtoUpdate>().ReverseMap();

            CreateMap<Lead, LeadDto>().ReverseMap();
            CreateMap<Lead, LeadDtoPost>().ReverseMap();
            CreateMap<Lead, LeadDtoUpdate>().ReverseMap();

            CreateMap<LeadAddress, LeadAddressDto>().ReverseMap();
            CreateMap<LeadAddress, LeadAddressPostDto>().ReverseMap();
            CreateMap<LeadAddress, LeadAddressUpdateDto>().ReverseMap();


            CreateMap<DemoStatus, DemoStatusDto>().ReverseMap();
            CreateMap<DemoStatus, DemoStatusDtoPost>().ReverseMap();
            CreateMap<DemoStatus, DemoStatusDtoUpdate>().ReverseMap();

            CreateMap<LeadStatus, LeadStatusDto>().ReverseMap();
            CreateMap<LeadStatus, LeadStatusDtoPost>().ReverseMap();
            CreateMap<LeadStatus, LeadStatusDtoUpdate>().ReverseMap();

            CreateMap<LeadType, LeadTypeDto>().ReverseMap();
            CreateMap<LeadType, LeadTypeDtoPost>().ReverseMap();
            CreateMap<LeadType, LeadTypeDtoUpdate>().ReverseMap();

            CreateMap<SecondarySource, SecondarySourceDto>().ReverseMap();
            CreateMap<SecondarySource, SecondarySourceDtoPost>().ReverseMap();
            CreateMap<SecondarySource, SecondarySourceDtoUpdate>().ReverseMap();

            CreateMap<Source, SourceDto>().ReverseMap();
            CreateMap<Source, SourceDtoPost>().ReverseMap();
            CreateMap<Source, SourceDtoUpdate>().ReverseMap();


            CreateMap<Inventory1, InventoyDto>().ReverseMap();

            CreateMap<Inventory_Transaction, Inventory_TransactionDto>().ReverseMap();

            CreateMap<EngineeringBom, ReleaseEnggBomDto>().ReverseMap();
            CreateMap<EngineeringBom, ReleaseEnggBomDtoPost>().ReverseMap();
            CreateMap<EngineeringBom, ReleaseEnggBomDtoUpdate>().ReverseMap();

            CreateMap<CostingBom, CostingBomDtoPost>().ReverseMap();
            CreateMap<CostingBom, CostingBomDto>().ReverseMap();
            CreateMap<CostingBom, CostingBomDtoUpdate>().ReverseMap();


            CreateMap<ProductionBom, ReleaseProductBomDtoPost>().ReverseMap();
            CreateMap<ProductionBom, ReleaseProductBomDtoUpdate>().ReverseMap();
            CreateMap<ProductionBom, ReleaseProductBomDto>().ReverseMap();


            CreateMap<EnggBomGroup, EnggBomGroupDto>().ReverseMap();
            CreateMap<EnggBomGroup, EnggBomGroupDtoPost>().ReverseMap();
            CreateMap<EnggBomGroup, EnggBomGroupDtoUpdate>().ReverseMap();
            CreateMap<EnggBomGroup, ListOfBomGroupDto>().ReverseMap();

            CreateMap<EnggCustomField, EnggCustomFieldDto>().ReverseMap();
            CreateMap<EnggCustomField, EnggCustomFieldDtoPost>().ReverseMap();
            CreateMap<EnggCustomField, EnggCustomFieldDtoUpdate>().ReverseMap();

            CreateMap<TypeSolution, TypeSolutionDto>().ReverseMap();
            CreateMap<TypeSolution, TypeSolutionPostDto>().ReverseMap();
            CreateMap<TypeSolution, TypeSolutionUpdateDto>().ReverseMap();

            CreateMap<ProductType, ProductTypeDto>().ReverseMap();
            CreateMap<ProductType, ProductTypePostDto>().ReverseMap();
            CreateMap<ProductType, ProductTypeUpdateDto>().ReverseMap();


            CreateMap<RoomNames, RoomNamesDto>().ReverseMap();
            CreateMap<RoomNames, RoomNamePostDto>().ReverseMap();
            CreateMap<RoomNames, RoomNameUpdateDto>().ReverseMap();

            CreateMap<FileUpload, FileUploadDto>().ReverseMap();
            CreateMap<FileUpload, FileUploadPostDto>().ReverseMap();
            CreateMap<FileUpload, FileUploadUpdateDto>().ReverseMap();

            CreateMap<ImageUpload, ImageUploadDto>().ReverseMap();
            CreateMap<ImageUpload, ImageUploadPostDto>().ReverseMap();
            CreateMap<ImageUpload, ImageUploadUpdateDto>().ReverseMap();

            CreateMap<BHK, BHKDto>().ReverseMap();
            CreateMap<BHK, BHKPostDto>().ReverseMap();
            CreateMap<BHK, BHKUpdateDto>().ReverseMap(); 

            CreateMap<State, StateDto>().ReverseMap();
            CreateMap<State, StatePostDto>().ReverseMap();
            CreateMap<State, StateUpdateDto>().ReverseMap(); 

            CreateMap<City, CityDto>().ReverseMap();
            CreateMap<City, CityPostDto>().ReverseMap();
            CreateMap<City, CityUpdateDto>().ReverseMap(); 

            CreateMap<Architectures, ArchitectureDto>().ReverseMap();
            CreateMap<Architectures, ArchitecturePostDto>().ReverseMap();
            CreateMap<Architectures, ArchitectureUpdateDto>().ReverseMap(); 

            CreateMap<PmcContractor, PmcContractorDto>().ReverseMap();
            CreateMap<PmcContractor, PmcContractorPostDto>().ReverseMap();
            CreateMap<PmcContractor, PmcContractorUpdateDto>().ReverseMap(); 

            CreateMap<LightningDesigner, LightningDesignerDto>().ReverseMap();
            CreateMap<LightningDesigner, LightningDesignerPostDto>().ReverseMap();
            CreateMap<LightningDesigner, LightningDesignerUpdateDto>().ReverseMap();

            CreateMap<StageOfConstruction, StageOfConstructionDto>().ReverseMap();
            CreateMap<StageOfConstruction, StageOfConstructionPostDto>().ReverseMap();
            CreateMap<StageOfConstruction, StageOfConstructionUpdateDto>().ReverseMap();

            CreateMap<TypeOfHome, TypeOfHomeDto>().ReverseMap();
            CreateMap<TypeOfHome, TypeOfHomePostDto>().ReverseMap();
            CreateMap<TypeOfHome, TypeOfHomeUpdateDto>().ReverseMap();

            CreateMap<ProjectName, ProjectNameDto>().ReverseMap();
            CreateMap<ProjectName, ProjectNamePostDto>().ReverseMap();
            CreateMap<ProjectName, ProjectNameUpdateDto>().ReverseMap();

            CreateMap<SourceDetails, SourceDetailsDto>().ReverseMap();
            CreateMap<SourceDetails, SourceDetailsPostDto>().ReverseMap();
            CreateMap<SourceDetails, SourceDetailsUpdateDto>().ReverseMap();

            CreateMap<SFT, SFTDto>().ReverseMap();
            CreateMap<SFT, SFTPostDto>().ReverseMap();
            CreateMap<SFT, SFTUpdateDto>().ReverseMap();

            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Role, RolePostDto>().ReverseMap();
            CreateMap<Role, RoleUpdateDto>().ReverseMap();

            CreateMap<RoleAccess, RoleAccessDto>().ReverseMap();
            CreateMap<RoleAccess, RoleAccessPostDto>().ReverseMap();
            CreateMap<RoleAccess, RoleAccessUpdateDto>().ReverseMap();

            CreateMap<RegistrationForm, RegistrationFormDto>().ReverseMap();
            CreateMap<RegistrationForm, RegistrationFormPostDto>().ReverseMap();
            CreateMap<RegistrationForm, RegistrationFormUpdateDto>().ReverseMap();

            CreateMap<UserAccess, UserAccessDto>().ReverseMap();
            CreateMap<UserAccess, UserAccessPostDto>().ReverseMap();
            CreateMap<UserAccess, UserAccessUpdateDto>().ReverseMap();

            CreateMap<FormsAccess, RoleAccessDto>().ReverseMap();
            CreateMap<FormsAccess, UserAccessDto>().ReverseMap();
        }
    }
}
