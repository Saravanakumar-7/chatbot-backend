using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCForServiceItems_ItemsDto
    {
        public int? Id { get; set; }
        public string? PONumber { get; set; }
        public int GrinsForServiceItemsPartsId { get; set; }
        public string? ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public string MftrItemNumber { get; set; }
        public List<GrinsForServiceItemsProjectNumbersDto>? GrinsForServiceItemsProjectNumbersDto { get; set; }
        public string ManufactureBatchNumber { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(18, 3)]
        public decimal POOrderedQty { get; set; }
        [Precision(18, 3)]
        public decimal POBalancedQty { get; set; }
        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }
        public string UOM { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class IQCForServiceItems_ItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public int GrinsForServiceItemsPartsId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class IQCForServiceItems_ItemsSaveDto
    {
        public string? ItemNumber { get; set; }
        public int GrinsForServiceItemsPartsId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
}
