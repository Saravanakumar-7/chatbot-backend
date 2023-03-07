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
        public string? ProjectNumber { get; set; }
        public OrderStatus StatusEnum { get; set; }

        [Precision(13, 3)]
        public decimal? BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal? DispatchQty { get; set; }

        [Precision(13, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal? OrderQty { get; set; }

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

         public string? Discount { get; set; }


        public DateTime RequestedDate { get; set; }
        public string? Remarks { get; set; }

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
        public decimal? ShopOrderQty { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal? OrderQty { get; set; }

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

         public string? Discount { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? Remarks { get; set; }
    }
    public class SalesOrderItemsUpdateDto
    {

        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
      //  public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
 
        [Precision(13, 3)]
        public decimal? BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal? DispatchQty { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }
         
        public string? Discount { get; set; }

        [Precision(13, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal? OrderQty { get; set; }

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
    public class InvoiceSalesOrderUpdateDispatchQtyDto
    {
        public string FGItemNumber { get; set; }
        public int SalesOrderId { get; set; }
        public decimal Qty { get; set; }

    }
    public class ItemDetailsForShopOrderDto
    {
        public string itemNumber { get; set; }
        public string itemType { get; set; }
        public decimal[] bomVersionNo { get; set; }
        public List<ProjectSODetailDto>? ProjectSODetails { get; set; }
    }
    public class ProjectSODetailDto
    {
        public string ProjectNumber { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerAliasName { get; set; }
        public List<SalesOrderQtyDto> SalesOrderQtyDetails { get; set; }
    }
    public class SalesOrderQtyDto
    {
        public string SalesOrderNo { get; set; }
        public string SalesOrderQty { get; set; }
        public string OpenSalesOrderQty { get; set; }
    }
}
