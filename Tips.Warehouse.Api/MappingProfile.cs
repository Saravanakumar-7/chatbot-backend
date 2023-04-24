using AutoMapper;
using System;
using System.Net;
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
            
            CreateMap<BTODeliveryOrder, BTODeliveryOrderDto>().ReverseMap();
            CreateMap<BTODeliveryOrder, BTODeliveryOrderDtoPost>().ReverseMap();
            CreateMap<BTODeliveryOrder, BTODeliveryOrderDtoUpdate>().ReverseMap();

            CreateMap<BTODeliveryOrderItems, BTODeliveryOrderItemsDto>().ReverseMap();
            CreateMap<BTODeliveryOrderItems, BTODeliveryOrderItemsDtoPost>().ReverseMap();
            CreateMap<BTODeliveryOrderItems, BTODeliveryOrderItemsDtoUpdate>().ReverseMap();

            CreateMap<BTOSerialNumber, BTOSerialNumberDto>().ReverseMap();
            CreateMap<BTOSerialNumber, BTOSerialNumberDtoPost>().ReverseMap();
            CreateMap<BTOSerialNumber, BTOSerialNumberDtoUpdate>().ReverseMap();

            CreateMap<DeliveryOrder, DeliveryOrderDto>().ReverseMap();
            CreateMap<DeliveryOrder, DeliveryOrderDtoPost>().ReverseMap();
            CreateMap<DeliveryOrder, DeliveryOrderDtoUpdate>().ReverseMap();

            CreateMap<DeliveryOrderItems, DeliveryOrderItemsDto>().ReverseMap();
            CreateMap<DeliveryOrderItems, DeliveryOrderItemsDtoPost>().ReverseMap();
            CreateMap<DeliveryOrderItems, DeliveryOrderItemsDtoUpdate>().ReverseMap();

            CreateMap<DoSerialNumber, DoSerialNumberDto>().ReverseMap();
            CreateMap<DoSerialNumber, DoSerialNumberDtoPost>().ReverseMap();
            CreateMap<DoSerialNumber, DoSerialNumberDtoUpdate>().ReverseMap();

            CreateMap<Inventory, InventoryDto>().ReverseMap();
            CreateMap<Inventory, InventoryDtoPost>().ReverseMap();
            CreateMap<Inventory, InventoryDtoUpdate>().ReverseMap();

            CreateMap<InventoryTranction, InventoryTranctionDto>().ReverseMap();
            CreateMap<InventoryTranction, InventoryTranctionDtoPost>().ReverseMap();
            CreateMap<InventoryTranction, InventoryTranctionDtoUpdate>().ReverseMap();

            CreateMap<BTODeliveryOrder, ListofBtoDeliveryOrderDetails>().ReverseMap();
            CreateMap<Inventory, GetInventoryListByItemNo>().ReverseMap();

            CreateMap<ReturnBtoDeliveryOrder, ReturnBtoDeliveryOrderDto>().ReverseMap();
            CreateMap<ReturnBtoDeliveryOrder, ReturnBtoDeliveryOrderPostDto>().ReverseMap();
            CreateMap<ReturnBtoDeliveryOrder, ReturnBtoDeliveryOrderUpdateDto>().ReverseMap();


            CreateMap<ReturnBtoDeliveryOrderItems, ReturnBtoDeliveryOrderItemsDto>().ReverseMap();
            CreateMap<ReturnBtoDeliveryOrderItems, ReturnBtoDeliveryOrderItemsPostDto>().ReverseMap();
            CreateMap<ReturnBtoDeliveryOrderItems, ReturnBtoDeliveryOrderItemsUpdateDto>().ReverseMap();

            CreateMap<BTODeliveryOrderItemsDtoPost, BtoDeliveryOrderDispatchQtyDetailsDto>().ReverseMap();

            CreateMap<ReturnBtoDeliveryOrderItemsPostDto, BtoDOReturnQtyDetailsDto>().ReverseMap();

            CreateMap<InvoiceChildItemPostDto, BtoDeliveryOrderInvoiceQtyDetailsDto>().ReverseMap();


            CreateMap<ReturnInvoice, ReturnInvoiceDto>().ReverseMap();
            CreateMap<ReturnInvoice, ReturnInvoiceDtoPost>().ReverseMap();
            CreateMap<ReturnInvoice, ReturnInvoiceDtoUpdate>().ReverseMap();

            CreateMap<ReturnInvoiceItem, ReturnInvoiceItemDto>().ReverseMap();
            CreateMap<ReturnInvoiceItem, ReturnInvoiceItemDtoPost>().ReverseMap();
            CreateMap<ReturnInvoiceItem, ReturnInvoiceItemDtoUpdate>().ReverseMap();

            CreateMap<ReturnInvoiceItemDtoPost, BtoInvoiceReturnQtyDetailsDto>().ReverseMap();

            CreateMap<BTODeliveryOrderHistory, ReturnBtoDeliveryOrderDto>().ReverseMap();

           
        





            //CreateMap<bTODeliveryOrderItems, inven>().ReverseMap();
        }
    }
}
