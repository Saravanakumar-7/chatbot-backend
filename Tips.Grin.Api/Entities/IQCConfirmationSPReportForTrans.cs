namespace Tips.Grin.Api.Entities
{
    public class IQCConfirmationSPReportForTrans
    {
        public string? GrinNumber { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? VendorName { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceValue { get; set; }
        public string? PONumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? GrinPartNo { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ManufactureBatchNumber { get; set; }
        public string? LotNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
        public string? UOM { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? GrinQty { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal? AcceptedQty { get; set; }
        public decimal? RejectedQty { get; set; }
        public int? Status { get; set; }
        public string? Remarks { get; set; }
        public decimal? TotalInvoiceValue { get; set; }
        public DateTime? IQCDate { get; set; }
        public string? AWBNumber1 { get; set; }
        public DateTime? AWBDate1 { get; set; }
        public decimal? ReceivedQty { get; set; }
        


    }
}
