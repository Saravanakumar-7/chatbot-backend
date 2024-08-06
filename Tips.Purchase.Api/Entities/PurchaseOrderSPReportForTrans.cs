namespace Tips.Purchase.Api.Entities
{
    public class PurchaseOrderSPReportForTrans
    {
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public int? RevisionNumber { get; set; }
        public string? Currency { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Transports { get; set; }
        public string? Other { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? POQnty { get; set; }
        public string? UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? PoStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? PRNumber { get; set; }
        public decimal? PRQty { get; set; }
        public DateTime? Scheduledate { get; set; }
        public decimal? Scheduleqnty { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public decimal? ConfirmationQty { get; set; }
    }
}
