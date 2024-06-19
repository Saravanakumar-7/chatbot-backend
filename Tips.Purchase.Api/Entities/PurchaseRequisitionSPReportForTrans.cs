namespace Tips.Purchase.Api.Entities
{
    public class PurchaseRequisitionSPReportForTrans
    {
        public string? PrNumber { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ProcurementType { get; set; }
        public byte? PrApprovalI { get; set; }
        public string? PrApprovedIBy { get; set; }
        public DateTime? PrApprovedIDate { get; set; }
        public byte? PrApprovalII { get; set; }
        public string? PrApprovedIIBy { get; set; }
        public string? ShippingMode { get; set; }
        public int? PrStatus { get; set; }
        public DateTime? PrApprovedIIDate { get; set; }
        public DateTime? prdate { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? UOM { get; set; }
        public decimal? Qty { get; set; }
        public string? ProjectNumber { get; set; }
    }
}
