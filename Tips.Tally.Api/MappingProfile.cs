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
              .ForMember(dest => dest.POData, opt => opt.Ignore())
              .ForMember(dest => dest.GSTIN_No, opt => opt.Ignore());

            CreateMap<TallyPurchaseOrderSpReportDto, TallyPurchaseOrderSpReport>()
                .ForMember(dest => dest.POData, opt => opt.Ignore())
                .ForMember(dest => dest.GSTIN_No, opt => opt.Ignore());



            CreateMap<TallyCustomerMasterSpReport, TallyCustomerMasterSpReportDto>()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore()) // Ignore JSON string
              .ReverseMap()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore());// Also ignore when reversing

            CreateMap<TallyVendorMasterSpReport, TallyVendorMasterSpReportDto>()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore()) // Ignore JSON string
              .ReverseMap()
              .ForMember(dest => dest.GSTINNo, opt => opt.Ignore());

            CreateMap<TallybtodeliveryorderSpReport, TallybtodeliveryorderSpReportDto>()
             .ForMember(dest => dest.SalesOrderData, opt => opt.Ignore()) // Ignore JSON string
             .ReverseMap()
             .ForMember(dest => dest.SalesOrderData, opt => opt.Ignore());

            CreateMap<TallyGrinSpReport, TallyGrinSpReportDto>()
             .ForMember(dest => dest.GRINParts, opt => opt.Ignore()) // Ignore JSON string
             .ReverseMap()
             .ForMember(dest => dest.GRINParts, opt => opt.Ignore());

            CreateMap<TallySalesOrderSpReport, TallySalesOrderSpReportDto>()
           .ForMember(dest => dest.SalesOrderItems, opt => opt.Ignore()) // Ignore JSON string
           .ReverseMap()
           .ForMember(dest => dest.SalesOrderItems, opt => opt.Ignore());

        }
    }
}
