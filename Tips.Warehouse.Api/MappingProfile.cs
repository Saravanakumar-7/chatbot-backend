using AutoMapper;
using System;
using System.Net;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;


namespace Tips.Warehouse.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InvoiceSPReport, InvoiceSPReportDTO>().ReverseMap();
            CreateMap<LocationTransferSPReport, LocationTransferSPReportDTO>().ReverseMap();
            CreateMap<OpenDeliveryOrderSPReport, OpenDeliveryOrderSPReportDto>().ReverseMap();
            CreateMap<ReturnDOSPReport, ReturnDOSPReportDTO>().ReverseMap();
            CreateMap<ReturnInvoiceSPResport, ReturnInvoiceSPReportDTO>().ReverseMap();

            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            CreateMap<Invoice, InvoicePostDto>().ReverseMap();
            CreateMap<Invoice, InvoiceUpdateDto>().ReverseMap();

            CreateMap<Invoice, DoNoInvoiceDto>().ReverseMap();
            CreateMap<InvoiceChildItem, DoNoInvoiceChildItemDto>().ReverseMap();
            CreateMap<InvoiceAdditionalCharges, DoNoInvoiceAdditionalChargesDto>().ReverseMap();

            CreateMap<BtoDeliveryOrderItemQtyDistribution, BtoDeliveryOrderItemQtyDistributionPostDto>().ReverseMap();
            CreateMap<BtoDeliveryOrderItemQtyDistribution, BtoDeliveryOrderItemQtyDistributionDto>().ReverseMap();

            CreateMap<ReturnInvoiceItemQtyDistribution, ReturnInvoiceItemQtyDistributionPostDto>().ReverseMap();
            CreateMap<ReturnInvoiceItemQtyDistribution, ReturnInvoiceItemQtyDistributionDto>().ReverseMap();

            CreateMap<ReturnDeliveryOrderItemQtyDistribution, ReturnDeliveryOrderItemQtyDistributionPostDto>().ReverseMap();
            CreateMap<ReturnDeliveryOrderItemQtyDistribution, ReturnDeliveryOrderItemQtyDistributionDto>().ReverseMap();

            CreateMap<ReturnOpenDeliveryOrderItemQtyDistribution, ReturnOpenDeliveryOrderItemQtyDistributionPostDto>().ReverseMap();
            CreateMap<ReturnOpenDeliveryOrderItemQtyDistribution, ReturnOpenDeliveryOrderItemQtyDistributionDto>().ReverseMap();

            CreateMap<OpenDeliveryOrderPartsQtyDistribution, OpenDeliveryOrderPartsQtyDistributionPostDto>().ReverseMap();
            CreateMap<OpenDeliveryOrderPartsQtyDistribution, OpenDeliveryOrderPartsQtyDistributionDto>().ReverseMap();

            CreateMap<InvoiceChildItem, InvoiceChildItemDto>().ReverseMap();
            CreateMap<InvoiceChildItem, InvoiceChildItemPostDto>().ReverseMap();
            CreateMap<InvoiceChildItem, InvoiceChildItemUpdateDto>().ReverseMap();

            CreateMap<OpenDeliveryOrder, OpenDeliveryOrderDto>().ReverseMap();
            CreateMap<OpenDeliveryOrder, OpenDeliveryOrderDtoPost>().ReverseMap();
            CreateMap<OpenDeliveryOrder, OpenDeliveryOrderDtoUpdate>().ReverseMap();

            CreateMap<ReturnOpenDeliveryOrder, ReturnOpenDeliveryOrderPostDto>().ReverseMap();
            CreateMap<ReturnOpenDeliveryOrder, ReturnOpenDeliveryOrderDto>().ReverseMap();
            CreateMap<ReturnOpenDeliveryOrder, ReturnOpenDeliveryOrderUpdateDto>().ReverseMap();

            CreateMap<ReturnOpenDeliveryOrderParts, ReturnOpenDeliveryOrderPartsPostDto>().ReverseMap();
            CreateMap<ReturnOpenDeliveryOrderParts, ReturnOpenDeliveryOrderPartsDto>().ReverseMap();
            CreateMap<ReturnOpenDeliveryOrderParts, ReturnOpenDeliveryOrderPartsUpdateDto>().ReverseMap();

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
            CreateMap<Inventory, InventoryGrinDtoPost>().ReverseMap();


            CreateMap<InventoryTranction, InventoryTranctionDto>().ReverseMap();
            CreateMap<InventoryTranction, InventoryTranctionDtoPost>().ReverseMap();
            CreateMap<InventoryTranction, InventoryTranctionDtoUpdate>().ReverseMap();
            CreateMap<InventoryTranction, InventoryTranctionGrinDtoPost>().ReverseMap();

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

            CreateMap<InvoiceChildItemPostDto, SalesOrderAdditionalChargesUpdate>().ReverseMap();

            CreateMap<ReturnInvoice, ReturnInvoiceDto>().ReverseMap();
            CreateMap<ReturnInvoice, ReturnInvoiceDtoPost>().ReverseMap();
            CreateMap<ReturnInvoice, ReturnInvoiceDtoUpdate>().ReverseMap();

            CreateMap<ReturnInvoiceItem, ReturnInvoiceItemDto>().ReverseMap();
            CreateMap<ReturnInvoiceItem, ReturnInvoiceItemDtoPost>().ReverseMap();
            CreateMap<ReturnInvoiceItem, ReturnInvoiceItemDtoUpdate>().ReverseMap();

            CreateMap<ReturnInvoiceItemDtoPost, BtoInvoiceReturnQtyDetailsDto>().ReverseMap();

            CreateMap<BTODeliveryOrderHistory, ReturnBtoDeliveryOrderDto>().ReverseMap();

            CreateMap<InvoiceAdditionalCharges, InvoiceAdditionalChargesDto>().ReverseMap();
            CreateMap<InvoiceAdditionalCharges, InvoiceAdditionalChargesPostDto>().ReverseMap();
            CreateMap<InvoiceAdditionalCharges, InvoiceAdditionalChargesUpdateDto>().ReverseMap();

            CreateMap<LocationTransfer, LocationTransferDto>().ReverseMap();
            CreateMap<LocationTransfer, LocationTransferPostDto>().ReverseMap();
            CreateMap<LocationTransfer, LocationTransferUpdateDto>().ReverseMap();

            CreateMap<ReturnODONumberListDto, BtoIDNameList>().ReverseMap();




            //CreateMap<Inventory, ConsumptionDto>().ReverseMap();



            //CreateMap<bTODeliveryOrderItems, inven>().ReverseMap();
        }
    }
}
