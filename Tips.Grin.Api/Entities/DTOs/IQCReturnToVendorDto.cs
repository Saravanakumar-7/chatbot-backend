using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tips.Grin.Api.Entities.Enums;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCReturnToVendorPostDto
    {
        public string? IQCNumber { get; set; }
        public string GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public List<IQCReturnToVendorItemsPostDto> iQCReturnToVendorItemsPostDtos { get; set; }
    }
    public class IQCReturnToVendorDto
    {
        public int Id { get; set; }
        public string? IQCNumber { get; set; }
        public string GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<IQCReturnToVendorItemsDto> iQCReturnToVendorItems { get; set; }
    }
    public class InventoryDtoDetails
    {
        public List<Datum> data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datum
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? UOM { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string Unit { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class InventoryUpdateDto
    {
        public string PartNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? UOM { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? SerialNo { get; set; }
        public string Unit { get; set; }
    }
    public class PurchaseOrderDtoDetails
    {
        public List<PurchaseOrderReturnsBackDto> data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
    public class PurchaseOrderReturnsBackDto
    {
        public string PurchaseOrderNo { get; set; }
        public List<PurchaseOrderReturnItemsBackDto> purchaseOrderItems { get; set; }
    }
    public class PurchaseOrderReturnItemsBackDto
    {
        public string ItemNumber { get; set; }
        public decimal ReturnQty { get; set; }
        public List<PurchaseOrderReturnProjectBackDto> purchaseOrderReturnProjectBackDtos { get; set; }
    }
    public class PurchaseOrderReturnProjectBackDto
    {
        public string ProjectNo { get; set; }
        public decimal ReturnQty { get; set; }
    }

}
