using AutoMapper;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            CreateMap<Invoice, InvoicePostDto>().ReverseMap();
            CreateMap<Invoice, InvoiceUpdateDto>().ReverseMap();

            CreateMap<InvoiceChildItem, InvoiceChildItemDto>().ReverseMap();
            CreateMap<InvoiceChildItem, InvoiceChildItemPostDto>().ReverseMap();
            CreateMap<InvoiceChildItem, InvoiceChildItemUpdateDto>().ReverseMap();

            CreateMap<OpenDeliveryOrder, OpenDeliveryOrderDto>().ReverseMap();
            CreateMap<OpenDeliveryOrder, OpenDeliveryOrderDtoPost>().ReverseMap();
            CreateMap<OpenDeliveryOrder, OpenDeliveryOrderDtoUpdate>().ReverseMap();


            CreateMap<OpenDeliveryOrderParts, OpenDeliveryOrderPartsDto>().ReverseMap();
            CreateMap<OpenDeliveryOrderParts, OpenDeliveryOrderPartsDtoPost>().ReverseMap();
            CreateMap<OpenDeliveryOrderParts, OpenDeliveryOrderPartsDtoUpdate>().ReverseMap();

        }
    }
}
