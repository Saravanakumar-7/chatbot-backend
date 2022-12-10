using AutoMapper;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
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

            CreateMap<SalesOrder, SalesOrderDto>().ReverseMap();
            CreateMap<SalesOrder, SalesOrderDtoPost>().ReverseMap();
            CreateMap<SalesOrder, SalesOrderDtoUpdate>().ReverseMap();

            CreateMap<SalesOrderItems, SalesOrderItemsDto>().ReverseMap();
            CreateMap<SalesOrderItems, SalesOrderItemsDtoPost>().ReverseMap();
            CreateMap<SalesOrderItems, SalesOrderItemsDtoUpdate>().ReverseMap();

            CreateMap<Quote, QuoteDto>().ReverseMap();
            CreateMap<Quote, QuoteDtoPost>().ReverseMap();
            CreateMap<Quote, QuoteDtoUpdate>().ReverseMap();

            CreateMap<QuoteAdditionalCharges, QuoteAdditionalChargesDto>().ReverseMap();
            CreateMap<QuoteAdditionalCharges, QuoteAdditionalChargesDtoPost>().ReverseMap();
            CreateMap<QuoteAdditionalCharges, QuoteAdditionalChargesDtoUpdate>().ReverseMap();

            CreateMap<QuoteGeneral, QuoteGeneralDto>().ReverseMap();
            CreateMap<QuoteGeneral, QuoteGeneralDtoPost>().ReverseMap();
            CreateMap<QuoteGeneral, QuoteGeneralDtoUpdate>().ReverseMap();

            CreateMap<QuoteOtherTerms, QuoteOtherTermsDto>().ReverseMap();
            CreateMap<QuoteOtherTerms, QuoteOtherTermsDtoPost>().ReverseMap();
            CreateMap<QuoteOtherTerms, QuoteOtherTermsDtoUpdate>().ReverseMap();

            CreateMap<QuoteRFQNotes, QuoteRFQNotesDto>().ReverseMap();
            CreateMap<QuoteRFQNotes, QuoteRFQNotesDtoPost>().ReverseMap();
            CreateMap<QuoteRFQNotes, QuoteRFQNotesDtoUpdate>().ReverseMap();

            CreateMap<QuoteSpecialTerms, QuoteSpecialTermsDto>().ReverseMap();
            CreateMap<QuoteSpecialTerms, QuoteSpecialTermsDtoPost>().ReverseMap();
            CreateMap<QuoteSpecialTerms, QuoteSpecialTermsDtoUpdate>().ReverseMap();
        }
    }
}
