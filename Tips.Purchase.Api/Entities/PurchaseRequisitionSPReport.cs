using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PurchaseRequisitionSPReport
    {
        public string? PrNumber { get; set; }

        public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }
        public bool PrApprovalI { get; set; } = false;
        public string? PrApprovedIBy { get; set; }
        public DateTime PrApprovedIDate { get; set; }

        public bool PrApprovalII { get; set; } = false;
        public string? PrApprovedIIBy { get; set; }
        public DateTime PrApprovedIIDate { get; set; }
        public string? ShippingMode { get; set; }
        public string? PrStatus { get; set; }
        public DateTime? prDate { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? UOM { get; set; }
        public decimal? Qty { get; set; }
    }
}
