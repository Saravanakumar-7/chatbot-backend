using AutoMapper;
using Tips.Tally.Api.Entities.DTOs;


namespace Tips.Tally.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<DiscountUsers, DiscountUsersUpdateDto>().ReverseMap();
            CreateMap<TallyPurchaseOrderSpReport, TallyPurchaseOrderSpReportDto>()
                .ForMember(dest => dest.POItems, opt => opt.Ignore()) // Ignore JSON string
                .ReverseMap()
                .ForMember(dest => dest.POItems, opt => opt.Ignore());

            CreateMap<TallyCustomerMasterSpReport, TallyCustomerMasterSpReportDto>()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore()) // Ignore JSON string
              .ReverseMap()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore());// Also ignore when reversing

            CreateMap<TallyVendorMasterSpReport, TallyVendorMasterSpReportDto>()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore()) // Ignore JSON string
              .ReverseMap()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore());

        }
    }
}
