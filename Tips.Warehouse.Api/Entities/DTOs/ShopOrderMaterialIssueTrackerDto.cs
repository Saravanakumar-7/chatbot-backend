using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ShopOrderMaterialIssueTrackerDto
    {
        public string ShopOrderNumber { get; set; }
        public string Description { get; set; }
        public string MRNumber { get; set; }
        public string PartNumber { get; set; }
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        [Precision(13, 3)]
        public decimal ConvertedToFgQty { get; set; }

        [Precision(13, 3)]
        public decimal Bomversion { get; set; }
        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }
        public decimal? WeightedAvg { get; set; }
        public string DataFrom { get; set; }
    }
    public class ShopOrderDtoForMaterialRequest
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string MRNumber { get; set; }
        public decimal IssueQty { get; set; }
        public string? DataFrom { get; set; }
        public string ShopOrderNumber { get; set; }
        public string Description { get; set; }
        public string MftrPartNumber { get; set; }
        public PartType PartType { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? UOM { get; set; }
        public string Unit { get; set; }

    }

    public class MRNIssueTrackerDto
    {
        public string PartNumber { get; set; }
        public decimal WipQty { get; set; }
        public string LotNumber { get; set; }
        public MaterialIssueData? MaterialIssueData { get; set; }
        public MaterialRequestData? MaterialRequestData { get; set; }
    }
    public class MaterialIssueData
    {
        public string PartNumber { get; set; }
        public string ShopOrderNumber { get; set; }
        public decimal QtyUsed { get; set; }
    }
    public class MaterialRequestData
    {
        public string PartNumber { get; set; }
        public string ShopOrderNumber { get; set; }
        public decimal QtyUsed { get; set; }
        public string MRNumber {  get; set; }
    }

    public class SomitConsumpWithBOMVersionDto
    {
        public string? InvoiceNumber { get; set; }
        public decimal InvoicedQty { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? BTONumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public decimal ShopOrderReleaseQty { get; set; }
        public decimal ShopOrderWipQty { get; set; }
        public string? PartNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? LotNumber { get; set; }
        public PartType? PartType { get; set; }
        public decimal? IssuedQty { get; set; }
        public decimal ConvertedToFgQty { get; set; }
        public DateTime? SomitDate { get; set; }
        public string? DataFrom { get; set; }
        public decimal Bomversion { get; set; }
        public decimal? BomQty { get; set; }
        public decimal? ConsumedQtyByInvoice { get; set; }
        public decimal? PPWipQty { get; set; }
    }
}
