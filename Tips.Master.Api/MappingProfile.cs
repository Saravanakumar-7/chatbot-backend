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
            CreateMap<VendorAddress, VendorAddressUpdateDto>().ReverseMap();

            CreateMap<VendorMaster, VendorMasterDto>().ReverseMap();
            CreateMap<VendorMaster, VendorMasterPostDto>().ReverseMap();
            CreateMap<VendorMaster, VendorMasterPostDto>().ReverseMap();



        }
    }
}
