namespace Tips.Purchase.Api.Entities
{
    public class Tras_POSPReport
    {
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? PartNumber { get; set; }
        public string? POApproval1Status { get; set; }
        public string? POApproval2Status { get; set; }
         
        public decimal? Qty { get; set; }
        public decimal? TotalWithTax { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }
        public decimal? BalanceQty { get; set; }
    }
}
