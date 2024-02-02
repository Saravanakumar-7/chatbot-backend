using Entities.Enums;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Entities.Dto
{
    public class SalesOrderItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? PriceList { get; set; }
        public decimal? AvailableStock { get; set; }

        public string? ProjectNumber { get; set; }
        public OrderStatus StatusEnum { get; set; }

        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }

        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public decimal ShopOrderReleaseQty { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal OrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }
        [Precision(13, 3)]
        public decimal? CGST { get; set; }
        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(13, 3)]
        public decimal? Discount { get; set; }
        public string? RoomName { get; set; }
        public string? DiscountType { get; set; }
        //public int SalesOrderId { get; set; }

        public DateTime RequestedDate { get; set; }
        public string? Remarks { get; set; }
        public List<ScheduleDateDto>? ScheduleDates { get; set; }
        public List<SoConfirmationDateDto> SoConfirmationDates { get; set; }

    }

    public class SalesOrderItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        //public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }


        //[Precision(13, 3)]
        //[DefaultValue(0)]
        //public decimal? DispatchQty { get; set; }

        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public string? PriceList { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal OrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }
        [Precision(13, 3)]
        public decimal? CGST { get; set; }
        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(13, 3)]
        public decimal? Discount { get; set; }
        public string? RoomName { get; set; }
        public string? DiscountType { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? Remarks { get; set; }
        public List<ScheduleDatePostDto>? ScheduleDates { get; set; }

    }
    public class SalesOrderItemsUpdateDto
    {

        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        //  public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(13, 3)]
        public decimal? Discount { get; set; }
        public string? RoomName { get; set; }
        public string? DiscountType { get; set; }

        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }
        public string? PriceList { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal OrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }
        [Precision(13, 3)]
        public decimal? CGST { get; set; }
        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? Remarks { get; set; }
        public List<ScheduleDateUpdateDto>? ScheduleDates { get; set; }

    }
    public class ListOfProjectNoDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
    }

    public class GetSalesOrderDetailsDto
    {
        public int Id { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? OrderQty { get; set; }
    }

    public class GetSalesOrderGSTListDto
    {
        public int SalesOrderId { get; set; }
        public string? ItemNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? IGST { get; set; }
    }
    public class SalesOrderDispatchQtyDto
    {
        public string FGItemNumber { get; set; }
        public int SalesOrderId { get; set; }
        public decimal DispatchQty { get; set; }

    }
    public class SalesOrderUpdateDispatchQtyDto
    {
        public string FGItemNumber { get; set; }
        public int SalesOrderId { get; set; }
        public decimal DispatchQty { get; set; }

    }

    public class ReturnDOSalesOrderDispatchQtyDto
    {
        public string FGPartNumber { get; set; }
        public int SalesOrderId { get; set; }
        public decimal ReturnQty { get; set; }

    }
    public class SoAdditionalChargeUpdateDto
    {
        public int SalesOrderId { get; set; }
        public decimal InvoicedValue { get; set; }
        public int SalesAdditionalChargeId { get; set; }
    }
    public class SARevisionNumber
    {
        public string ItemNumber { get; set; }
        public List<string> FGItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public decimal[] BomVersionNo { get; set; }
        public List<ProjectSOSADetailDto>? ProjectSODetails { get; set; }

    }

    public class ShopOrderReleaseQtyDto
    {
        public string? FGItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal ReleaseQty { get; set; }

    }
    public class UpdatePendingShopOrderConfirmationQtyDto
    {
        public string? FGItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal PendingSoConfirmationQty { get; set; }
    }
    public class ItemDetailsForShopOrderDto
    {
        public string ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public decimal[] BomVersionNo { get; set; }
        public List<ProjectSODetailDto>? ProjectSODetails { get; set; }
    }
    public class ItemdetailsDto
    {
        public string itemNumber { get; set; }
        public string projectType { get; set; }
    }
    public class ItemDetailsForShopOrderBomDto
    {
        public string ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public string? BomVersionNo { get; set; }
        public List<ProjectSODetailDto>? ProjectSODetails { get; set; }
    }
    public class ProjectSODetailDto
    {
        public string ProjectNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public List<SalesOrderQtyDto> SalesOrderQtyDetails { get; set; }
    }

    public class ProjectSOSADetailDto
    {
        public string ProjectNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public List<SalesOrderQtyForSADto> SalesOrderQtyDetails { get; set; }
    }

    public class SalesOrderQtyDto
    {
        public string SalesOrderNo { get; set; }
        public decimal SalesOrderQty { get; set; }
        public decimal OpenSalesOrderQty { get; set; }
    }
    public class SalesOrderQtyForSADto
    {
        public string SalesOrderNo { get; set; }
        public decimal SalesOrderQty { get; set; }
        public decimal OpenSalesOrderQty { get; set; }
        public decimal RequiredQty { get; set; }
        public string Description { get; set; }
        public string FgItemNumber { get; set; }
        public string ProjectNumber { get; set; }
    }
    public class SalesOrderItemsReportDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? PriceList { get; set; }
        public decimal? AvailableStock { get; set; }

        public string? ProjectNumber { get; set; }
        public OrderStatus StatusEnum { get; set; }

        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }

        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal OrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }
        [Precision(13, 3)]
        public decimal? CGST { get; set; }
        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(13, 3)]
        public decimal? Discount { get; set; }
        public string? RoomName { get; set; }
        public string? DiscountType { get; set; }
        //public int SalesOrderId { get; set; }

        public DateTime RequestedDate { get; set; }
        public string? Remarks { get; set; }
        public List<ScheduleDateReportDto>? ScheduleDates { get; set; }

    }
    public class SalesOrderFGandBalanceQty
    {
        public string? FGItemNumber { get; set; }
        public decimal Balance_Qty { get; set; }
        public PartType PartType { get; set; }


    }
    public class SoItemConfirmationDateDto
    {
        public int SalesOrderId { get; set; }
        public int SalesOrderItemsId { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public decimal Qty { get; set; }
    }

    public class ProjectDetailDto
    {
        public string FGItemNumber { get; set; }
        public List<ProjectDetailDto> ProjectDetailDtos { get; set; }
    }

    public class InventoryItemdetailsDto
    {
        public Datum[] data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string partNumber { get; set; }
        public string lotNumber { get; set; }
        public string mftrPartNumber { get; set; }
        public string description { get; set; }
        public string projectNumber { get; set; }
        public int balance_Quantity { get; set; }
        public string uom { get; set; }
        public bool isStockAvailable { get; set; }
        public string warehouse { get; set; }
        public string location { get; set; }
        public string unit { get; set; }
        public string grinNo { get; set; }
        public int? grinPartId { get; set; }
        public int partType { get; set; }
        public string grinMaterialType { get; set; }
        public string referenceID { get; set; }
        public string referenceIDFrom { get; set; }
        public string shopOrderNo { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public string lastModifiedBy { get; set; }
        public DateTime? lastModifiedOn { get; set; }
    }

}
