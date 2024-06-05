using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities.DTOs
{
    public class OQCBinningDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public List<OQCBinningLocationDto>? oQCBinningLocations { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCBinningPostDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public List<OQCBinningLocationPostDto>? oQCBinningLocations { get; set; }
    }
    public class OQCBinningUpdateDto 
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public List<OQCBinningLocationUpdateDto>? oQCBinningLocations { get; set; }

    }
    public class OQCBinningLocationPostDto
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string? SerialNo { get; set; }
    }
    public class OQCBinningLocationDto
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string? SerialNo { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCBinningLocationUpdateDto
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string? SerialNo { get; set; }
    }
    public class OQCBinningInventoryDto
    {
        public List<Data>? data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
    public class Data
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
        public string? Unit { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCBinningInventoryUpdateDto
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
        public string? Unit { get; set; }
    }
    public class OQCBinningSPReportDto
    {
        public string? ItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
    }
}
