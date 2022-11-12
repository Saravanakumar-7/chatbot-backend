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



        }
    }
}
