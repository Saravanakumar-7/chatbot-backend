namespace Tips.Purchase.Api.Entities
{
    public class PoProjectSPReport
    {
            public string? VendorId { get; set; }
            public string? VendorName { get; set; }
            public string? PONumber { get; set; }
            public DateTime? PODate { get; set; }
            //public string? PRNumber { get; set; }
            public decimal? PRQty { get; set; }
            public int? RevisionNumber { get; set; }
            public string? ProjectNumber { get; set; }
            public decimal? ProjectQty { get; set; }
            public string? ItemNumber { get; set; }
            public string? MftrItemNumber { get; set; }
            public string? ItemDescription { get; set; }
            public decimal? POQnty { get; set; }
            public decimal? ReceivedQty { get; set; }
            public decimal? BalanceQty { get; set; }
            public string? Currency { get; set; }
            public string? UOM { get; set; }
            public decimal? UnitPrice { get; set; }
            public string? PaymentTerms { get; set; }
            public decimal? BalanceValue { get; set; }
            public string? POApprovedIBy { get; set; }
            public DateTime? POApprovedIDate { get; set; }
            public string? POApprovedIIBy { get; set; }
            public DateTime? POApprovedIIDate { get; set; }
            public int? PoStatus { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime? CreatedOn { get; set; }
    }
}
