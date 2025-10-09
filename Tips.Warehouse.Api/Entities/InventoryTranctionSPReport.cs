using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities
{
    public class InventoryTranctionSPReport
    {
        public string? PartNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? Issued_Quantity { get; set; }
        public string? UOM { get; set; }
        public DateTime? Issued_DateTime { get; set; }
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public decimal? BOM_Version_No { get; set; }
        public string? From_Location { get; set; }
        public string? TO_Location { get; set; }
        public bool? ModifiedStatus { get; set; } = false;
        public string? Unit { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public PartType? PartType { get; set; }
        public string? LotNumber { get; set; }
        public bool IsStockAvailable { get; set; }
        public string? Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
    }

    public class InventoryTranctioninternalSPReport
    {
        public string? PartNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? Issued_Quantity { get; set; }
        public string? UOM { get; set; }
        public DateTime? Issued_DateTime { get; set; }
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public decimal? BOM_Version_No { get; set; }
        public string? From_Location { get; set; }
        public string? TO_Location { get; set; }
        public bool? ModifiedStatus { get; set; } = false;
        public string? Unit { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public PartType? PartType { get; set; }
        public string? LotNumber { get; set; }
        public bool IsStockAvailable { get; set; }
        public string? Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
    }

    public class InventoryTranctioninternalinputparam{
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

    }
    public class InventoryTranctioninternalinputparamForExcelDto
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
