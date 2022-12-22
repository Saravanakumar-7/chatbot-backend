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
            CreateMap<RfqCustomerSupport, RfqCustomerSupportDto>().ReverseMap();
            CreateMap<RfqCustomerSupport, RfqCustomerSupportPostDto>().ReverseMap();
            CreateMap<RfqCustomerSupport, RfqCustomerSupportUpdateDto>().ReverseMap();
            CreateMap<RfqCustomerSupport, RfqCustomerSupportUpdateReleaseDto>().ReverseMap();


            CreateMap<RfqCSDeliverySchedule, RfqCSDeliveryScheduleDto>().ReverseMap();
            CreateMap<RfqCSDeliverySchedule, RfqCSDeliverySchedulePostDto>().ReverseMap();
            CreateMap<RfqCSDeliverySchedule, RfqCSDeliveryScheduleUpdateDto>().ReverseMap();

            CreateMap<RfqCustomerSupportItems, RfqCustomerSupportItemDto>().ReverseMap();
            CreateMap<RfqCustomerSupportItems, RfqCustomerSupportItemPostDto>().ReverseMap();
            CreateMap<RfqCustomerSupportItems, RfqCustomerSupportItemUpdateDto>().ReverseMap();
            CreateMap<RfqCustomerSupportItems, RfqCustomerSupportItemUpdateReleaseDto>().ReverseMap();

            

            CreateMap<RfqCustomerSupportNotes, RfqCustomerSupportNotesDto>().ReverseMap();
            CreateMap<RfqCustomerSupportNotes, RfqCustomerSupportNotesPostDto>().ReverseMap();
            CreateMap<RfqCustomerSupportNotes, RfqCustomerSupportNotesUpdateDto>().ReverseMap();

            CreateMap<Rfq, RfqDto>().ReverseMap();
            CreateMap<Rfq, RfqPostDto>().ReverseMap();
            CreateMap<Rfq, RfqUpdateDto>().ReverseMap();
            CreateMap<Rfq, RfqNumberListDto>().ReverseMap();


            CreateMap<RfqCustomGroup, RfqCustomGroupDto>().ReverseMap();
            CreateMap<RfqCustomGroup, RfqCustomGroupPostDto>().ReverseMap();
            CreateMap<RfqCustomGroup, RfqCustomGroupUpdateDto>().ReverseMap();

             
            CreateMap<SourcingVendor, SourcingVendorDto>().ReverseMap();
            CreateMap<SourcingVendor, SourcingVendorPostDto>().ReverseMap();
            CreateMap<SourcingVendor, SourcingVendorUpdateDto>().ReverseMap();

            CreateMap<RfqSourcing, RfqSourcingDto>().ReverseMap();
            CreateMap<RfqSourcing, RfqSourcingDtoPost>().ReverseMap();
            CreateMap<RfqSourcing, RfqSourcingDtoUpdate>().ReverseMap();

            CreateMap<RfqSourcingItems, RfqSourcingItemsDto>().ReverseMap();
            CreateMap<RfqSourcingItems, RfqSourcingItemsDtoPost>().ReverseMap();
            CreateMap<RfqSourcingItems, RfqSourcingItemsDtoUpdate>().ReverseMap();

            CreateMap<RfqSourcingVendor, RfqSourcingVendorDto>().ReverseMap();
            CreateMap<RfqSourcingVendor, RfqSourcingVendorDtoPost>().ReverseMap();
            CreateMap<RfqSourcingVendor, RfqSourcingVendorDtoUpdate>().ReverseMap();


            CreateMap<RfqEngg, RfqEnggDto>().ReverseMap();
            CreateMap<RfqEngg, RfqEnggDtoPost>().ReverseMap();
            CreateMap<RfqEngg, RfqEnggDtoUpdate>().ReverseMap();

            CreateMap<RfqEnggItem, RfqEnggItemDto>().ReverseMap();
            CreateMap<RfqEnggItem, RfqEnggItemDtoPost>().ReverseMap();
            CreateMap<RfqEnggItem, RfqEnggItemDtoUpdate>().ReverseMap();

            CreateMap<RfqEnggRiskIdentification, RfqEnggRiskIdentificationDto>().ReverseMap();
            CreateMap<RfqEnggRiskIdentification, RfqEnggRiskIdentificationDtoPost>().ReverseMap();
            CreateMap<RfqEnggRiskIdentification, RfqEnggRiskIdentificationDtoUpdate>().ReverseMap();

            CreateMap<RfqLPCosting, RfqLPCostingDto>().ReverseMap();
            CreateMap<RfqLPCosting, RfqLPCostingDtoPost>().ReverseMap();
            CreateMap<RfqLPCosting, RfqLPCostingDtoUpdate>().ReverseMap();

            CreateMap<RfqLPCostingItem, RfqLPCostingItemDto>().ReverseMap();
            CreateMap<RfqLPCostingItem, RfqLPCostingItemDtoPost>().ReverseMap();
            CreateMap<RfqLPCostingItem, RfqLPCostingItemDtoUpdate>().ReverseMap();

            CreateMap<RfqLPCostingProcess, RfqLPCostingProcessDto>().ReverseMap();
            CreateMap<RfqLPCostingProcess, RfqLPCostingProcessDtoPost>().ReverseMap();
            CreateMap<RfqLPCostingProcess, RfqLPCostingProcessDtoUpdate>().ReverseMap();

            CreateMap<RfqLPCostingNREConsumable, RfqLPCostingNREConsumableDto>().ReverseMap();
            CreateMap<RfqLPCostingNREConsumable, RfqLPCostingNREConsumableDtoPost>().ReverseMap();
            CreateMap<RfqLPCostingNREConsumable, RfqLPCostingNREConsumableDtoUpdate>().ReverseMap();

            CreateMap<RfqLPCostingOtherCharges, RfqLPCostingOtherChargesDto>().ReverseMap();
            CreateMap<RfqLPCostingOtherCharges, RfqLPCostingOtherChargesDtoPost>().ReverseMap();
            CreateMap<RfqLPCostingOtherCharges, RfqLPCostingOtherChargesDtoUpdate>().ReverseMap();

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


            CreateMap<FgOqc, FgOqcDto>().ReverseMap();
            CreateMap<FgOqc, FgOqcDtoPost>().ReverseMap();
            CreateMap<FgOqc, FgOqcDtoUpdate>().ReverseMap();


            CreateMap<SaOqc, SaOqcDto>().ReverseMap();
            CreateMap<SaOqc, SaOqcDtoPost>().ReverseMap();
            CreateMap<SaOqc, SaOqcDtoUpdate>().ReverseMap();

            CreateMap<ForeCast, ForeCastDto>().ReverseMap();
            CreateMap<ForeCast, ForeCastPostDto>().ReverseMap();
            CreateMap<ForeCast, ForeCastUpdateDto>().ReverseMap();
            CreateMap<ForeCast, ForeCastNumberListDto>().ReverseMap();

            CreateMap<ForeCastCustomerSupport, ForeCastCustomerSupportDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupport, ForeCastCustomerSupportPostDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupport, ForeCastCustomerSupportUpdateDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupport, ForeCAstCustomerSupportUpdateReleaseDto>().ReverseMap();


            CreateMap<ForeCastCSDeliverySchedule, ForeCastCSDeliveryScheduleDto>().ReverseMap();
            CreateMap<ForeCastCSDeliverySchedule, ForeCastCSDeliverySchedulePostDto>().ReverseMap();
            CreateMap<ForeCastCSDeliverySchedule, ForeCastCSDeliveryScheduleUpdateDto>().ReverseMap();

            CreateMap<ForeCastCustomerSupportItem, ForeCastCustomerSupportItemDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupportItem, ForeCastCustomerSupportItemPostDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupportItem, ForeCastCustomerSupportItemUpdateDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupportItem, ForeCAstCustomerSupportUpdateReleaseDto>().ReverseMap();



            CreateMap<ForeCastCustomerSupportNotes, ForeCastCustomerSupportNotesDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupportNotes, ForeCastCustomerSupportNotesPostDto>().ReverseMap();
            CreateMap<ForeCastCustomerSupportNotes, ForeCastCustomerSupportNotesUpdateDto>().ReverseMap();

            CreateMap<ForeCastEngg, ForeCastEnggDto>().ReverseMap();
            CreateMap<ForeCastEngg, ForeCastEnggPostDto>().ReverseMap();
            CreateMap<ForeCastEngg, ForeCastEnggUpdateDto>().ReverseMap();

            CreateMap<ForeCastEnggItems, ForeCastEnggItemsDto>().ReverseMap();
            CreateMap<ForeCastEnggItems, ForeCastEnggItemsPostDto>().ReverseMap();
            CreateMap<ForeCastEnggItems, ForeCastEnggItemsUpdateDto>().ReverseMap();

            CreateMap<ForeCastEnggRiskIdentification, ForeCastEnggRiskIdentificationDto>().ReverseMap();
            CreateMap<ForeCastEnggRiskIdentification, ForeCastEnggRiskIdentificationPostDto>().ReverseMap();
            CreateMap<ForeCastEnggRiskIdentification, ForeCastEnggRiskIdentificationUpdateDto>().ReverseMap();


            CreateMap<ForecastSourcing, ForecastSourcingDto>().ReverseMap();
            CreateMap<ForecastSourcing, ForecastSourcingDtoPost>().ReverseMap();
            CreateMap<ForecastSourcing, ForecastSourcingDtoUpdate>().ReverseMap();

            CreateMap<ForecastSourcingItems, ForecastSourcingItemsDto>().ReverseMap();
            CreateMap<ForecastSourcingItems, ForecastSourcingItemsDtoPost>().ReverseMap();
            CreateMap<ForecastSourcingItems, ForecastSourcingItemsDtoUpdate>().ReverseMap();

            CreateMap<ForecastSourcingVendor, ForecastSourcingVendorDto>().ReverseMap();
            CreateMap<ForecastSourcingVendor, ForecastSourcingVendorDtoPost>().ReverseMap();
            CreateMap<ForecastSourcingVendor, ForecastSourcingVendorDtoUpdate>().ReverseMap();

            CreateMap<ForecastLpCosting, ForecastLpCostingDto>().ReverseMap();
            CreateMap<ForecastLpCosting, ForecastLPCostingDtoPost>().ReverseMap();
            CreateMap<ForecastLpCosting, ForecastLPCostingDtoUpdate>().ReverseMap();

            CreateMap<ForecastLpCostingItem, ForecastLpCostingItemDto>().ReverseMap();
            CreateMap<ForecastLpCostingItem, ForecastLPCostingItemDtoPost>().ReverseMap();
            CreateMap<ForecastLpCostingItem, ForecastLPCostingItemDtoUpdate>().ReverseMap();

            CreateMap<ForecastLpCostingProcess, ForecastLpCostingProcessDto>().ReverseMap();
            CreateMap<ForecastLpCostingProcess, ForecastLPCostingProcessDtoPost>().ReverseMap();
            CreateMap<ForecastLpCostingProcess, ForecastLPCostingProcessDtoUpdate>().ReverseMap();



            CreateMap<ForecastLPCostingNREConsumable, ForecastLpCostingNREConsumableDto>().ReverseMap();
            CreateMap<ForecastLPCostingNREConsumable, ForecastLPCostingNREConsumableDtoPost>().ReverseMap();
            CreateMap<ForecastLPCostingNREConsumable, ForecastLPCostingNREConsumableDtoUpdate>().ReverseMap();

            CreateMap<ForecastLpCostingOtherCharges, ForecastLpCostingOtherChargesDto>().ReverseMap();
            CreateMap<ForecastLpCostingOtherCharges, ForecastLPCostingOtherChargesDtoPost>().ReverseMap();
            CreateMap<ForecastLpCostingOtherCharges, ForecastLPCostingOtherChargesDtoUpdate>().ReverseMap();

            CreateMap<RfqCustomField, RfqCustomFieldDto>().ReverseMap();
            CreateMap<RfqCustomField, RfqCustomFieldDtoPost>().ReverseMap();
            CreateMap<RfqCustomField, RfqCustomFieldDtoUpdate>().ReverseMap();

            CreateMap<ForeCastCustomField, ForeCastCustomFieldDto>().ReverseMap();
            CreateMap<ForeCastCustomField, ForeCastCustomFieldDtoPost>().ReverseMap();
            CreateMap<ForeCastCustomField, ForeCastCustomFieldDtoUpdate>().ReverseMap();

            CreateMap<ForeCastCustomGroup, ForeCastCustomGroupDto>().ReverseMap();
            CreateMap<ForeCastCustomGroup, ForeCastCustomGroupDtoPost>().ReverseMap();
            CreateMap<ForeCastCustomGroup, ForeCastCustomGroupDtoUpdate>().ReverseMap();
        }
    }
}
