using AutoMapper;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Rfq, RfqDto>().ReverseMap();
            CreateMap<Rfq, RfqPostDto>().ReverseMap();
            CreateMap<Rfq, RfqUpdateDto>().ReverseMap();

            CreateMap<RfqCustomGroup, RfqCustomGroupDto>().ReverseMap();
            CreateMap<RfqCustomGroup, RfqCustomGroupPostDto>().ReverseMap();
            CreateMap<RfqCustomGroup, RfqCustomGroupUpdateDto>().ReverseMap();

            CreateMap<RfqCustomerSupport, RfqCustomerSupportDto>().ReverseMap();
            CreateMap<RfqCustomerSupport, RfqCustomerSupportPostDto>().ReverseMap();
            CreateMap<RfqCustomerSupport, RfqCustomerSupportUpdateDto>().ReverseMap();
             
            CreateMap<SourcingVendor, SourcingVendorDto>().ReverseMap();
            CreateMap<SourcingVendor, SourcingVendorPostDto>().ReverseMap();
            CreateMap<SourcingVendor, SourcingVendorUpdateDto>().ReverseMap();

            CreateMap<RfqSourcing, RfqSourcingDto>().ReverseMap();
            CreateMap<RfqSourcing, RfqSourcingDtoPost>().ReverseMap();
            CreateMap<RfqSourcing, RfqSourcingDtoUpdate>().ReverseMap();

            CreateMap<RfqSourcingItems, RfqSourcingItemsDto>().ReverseMap();
            CreateMap<RfqSourcingItems, RfqSourcingItemsDtoPost>().ReverseMap();
            CreateMap<RfqSourcingItems, RfqSourcingItemsDtoUpdate>().ReverseMap();
        }
    }
}
