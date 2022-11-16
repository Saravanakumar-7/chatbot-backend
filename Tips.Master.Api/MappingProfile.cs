using AutoMapper;
using Entities;
using Entities.DTOs;

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
            CreateMap<CustomerBanking, CustomerContactsDtoUpdate>().ReverseMap();


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

            CreateMap<BasicOfApproval, BasicOfApprovalDto>().ReverseMap();
            CreateMap<BasicOfApproval, BasicOfApprovalPostDto>().ReverseMap();
            CreateMap<BasicOfApproval, BasicOfApprovalUpdateDto>().ReverseMap();
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

            CreateMap<HeadCounting, HeadCountingDto>().ReverseMap();
            CreateMap<HeadCounting, HeadCountingPostDto>().ReverseMap();
            CreateMap<HeadCounting, HeadCountingUpdateDto>().ReverseMap();



        }
    }
}
