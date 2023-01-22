using Microsoft.Build.Framework;
using System.ComponentModel;
using Tips.SalesService.Api.Entities.Dto;

namespace Tips.SalesService.Api.Entities.Dto
{
    public class SalesOrderItemsDto
    {
        public int Id { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public decimal UOM { get; set; }
        public decimal? BalanceQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? ShopOrderQty { get; set; } 
        public decimal Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remarks { get; set; }
       
    }

    public class SalesOrderItemsDtoPost
    {
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public decimal UOM { get; set; }
        public decimal Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remarks { get; set; }
       
    }
    public class SalesOrderItemsDtoUpdate
    {
     
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public decimal UOM { get; set; } 
        public decimal Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remarks { get; set; }
        
    }
    public class ListOfProjectNoDto
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
    }

    public class GetSalesOrderDetailsDto
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public decimal OrderQty { get; set; }
    }
}
