namespace Tips.Purchase.Api.Entities
{
    public class PurchaseRequisitionSPReportForTrans
    {
        public string? PrNumber { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ProcurementType { get; set; }
        public DateTime? Prdate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal? PRScheduledQty { get; set; }
        public DateTime? PRScheduledDate { get; set; }
        public bool? PrApprovalI { get; set; }
        public string? PrApprovedIBy { get; set; }
        public DateTime? PrApprovedIDate { get; set; }
        public bool? PrApprovalII { get; set; }
        public string? PrApprovedIIBy { get; set; }
        public DateTime? PrApprovedIIDate { get; set; }
        public int? PrStatus { get; set; }
        public string? PrCreatedBy { get; set; }
        public string? DrawingRevisionNumber { get; set; }

    }
}
