using Entities;
using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnOpenDeliveryOrderPartsDto
    {
        [Key]
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string? ODONumber { get; set; }
        public string Description { get; set; }
        public PartType ItemType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal AlreadyReturnQty { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        public decimal StockAvailable { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? LocationStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string Remarks { get; set; }
        public string? SerialNo { get; set; }
        public List<ReturnOpenDeliveryOrderItemQtyDistributionDto>? QtyDistribution { get; set; }
    }
    public class ReturnOpenDeliveryOrderPartsPostDto
    {
        public int ODOPartId { get; set; }
        public string ItemNumber { get; set; }
        public string? ODONumber { get; set; }
        public string Description { get; set; }
        public PartType ItemType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal AlreadyReturnQty { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        public decimal StockAvailable { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? LocationStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string? Remarks { get; set; }
        public string? SerialNo { get; set; }
        public List<ReturnOpenDeliveryOrderItemQtyDistributionPostDto>? QtyDistribution { get; set; }
    }
    public class ReturnOpenDeliveryOrderPartsUpdateDto
    {
        [Key]
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string? ODONumber { get; set; }
        public string Description { get; set; }
        public PartType ItemType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal AlreadyReturnQty { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        public decimal StockAvailable { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? LocationStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string Remarks { get; set; }
        public string? SerialNo { get; set; }
    }
}
