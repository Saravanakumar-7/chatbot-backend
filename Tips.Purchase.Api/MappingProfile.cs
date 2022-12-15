using AutoMapper;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PurchaseOrder, PurchaseOrderDto>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderDtoPost>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderDtoUpdate>().ReverseMap();

            CreateMap<PoItem, PoItemsDto>().ReverseMap();
            CreateMap<PoItem, PoItemsDtoPost>().ReverseMap();
            CreateMap<PoItem, PoItemsDtoUpdate>().ReverseMap();

            CreateMap<PoAddProject, PoAddProjectDto>().ReverseMap();
            CreateMap<PoAddProject, PoAddProjectDtoPost>().ReverseMap();
            CreateMap<PoAddProject, PoAddProjectDtoUpdate>().ReverseMap();

            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleDto>().ReverseMap();
            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleDtoPost>().ReverseMap();
            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleDtoUpdate>().ReverseMap();

       

            CreateMap<PurchaseRequisition, PurchaseRequisitionDto>().ReverseMap();
            CreateMap<PurchaseRequisition, PurchaseRequisitionDtoPost>().ReverseMap();
            CreateMap<PurchaseRequisition, PurchaseRequisitionDtoUpdate>().ReverseMap();

            CreateMap<PrItem, PrItemsDto>().ReverseMap();
            CreateMap<PrItem, PrItemsDtoPost>().ReverseMap();
            CreateMap<PrItem, PrItemsDtoUpdate>().ReverseMap();

            CreateMap<PrAddProject, PrAddProjectDto>().ReverseMap();
            CreateMap<PrAddProject, PrAddProjectDtoPost>().ReverseMap();
            CreateMap<PrAddProject, PrAddProjectDtoUpdate>().ReverseMap();

            CreateMap<PrAddDeliverySchedule, PrAddDeliveryScheduleDto>().ReverseMap();
            CreateMap<PrAddDeliverySchedule, PrAddDeliveryScheduleDtoPost>().ReverseMap();
            CreateMap<PrAddDeliverySchedule, PrAddDeliveryScheduleDtoUpdate>().ReverseMap();


        }
    }
}
