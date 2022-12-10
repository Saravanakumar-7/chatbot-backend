using AutoMapper;
using Tips.SalesService.Api.Entities;
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

            CreateMap<RfqCSDeliverySchedule, RfqCSDeliveryScheduleDto>().ReverseMap();
            CreateMap<RfqCSDeliverySchedule, RfqCSDeliverySchedulePostDto>().ReverseMap();
            CreateMap<RfqCSDeliverySchedule, RfqCSDeliveryScheduleUpdateDto>().ReverseMap();

            CreateMap<RfqCustomerSupportItems, RfqCustomerSupportItemDto>().ReverseMap();
            CreateMap<RfqCustomerSupportItems, RfqCustomerSupportItemPostDto>().ReverseMap();
            CreateMap<RfqCustomerSupportItems, RfqCustomerSupportItemUpdateDto>().ReverseMap();

            CreateMap<RfqCustomerSupportNotes, RfqCustomerSupportNotesDto>().ReverseMap();
            CreateMap<RfqCustomerSupportNotes, RfqCustomerSupportNotesPostDto>().ReverseMap();
            CreateMap<RfqCustomerSupportNotes, RfqCustomerSupportNotesUpdateDto>().ReverseMap();

            CreateMap<Rfq, RfqDto>().ReverseMap();
            CreateMap<Rfq, RfqPostDto>().ReverseMap();
            CreateMap<Rfq, RfqUpdateDto>().ReverseMap();

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
        }
    }
}
