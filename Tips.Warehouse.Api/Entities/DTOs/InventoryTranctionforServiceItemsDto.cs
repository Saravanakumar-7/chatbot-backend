using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InventoryTranctionforServiceItemsPostDto
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public PartType PartType { get; set; }
        public string ProjectNumber { get; set; }
        public InventoryType TransactionType { get; set; }
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }
        public string? UOM { get; set; }
        public string? ShopOrderId { get; set; }
        public string ReferenceID { get; set; }
        public string ReferenceIDFrom { get; set; }
        public decimal? BOM_Version_No { get; set; }
        public string? From_Location { get; set; }
        public string TO_Location { get; set; }
        public bool? ModifiedStatus { get; set; } = false;
        public string? Unit { get; set; }
        public string? GrinsForServiceItemsMaterialType { get; set; }
        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinsForServiceItemsNumber { get; set; }
        public int? GrinsForServiceItemsPartsId { get; set; }
        public string? shopOrderNo { get; set; }
    }
}
