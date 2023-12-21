namespace Tips.Purchase.Api.Entities
{
    public class PurchaseOrderSPReport
    {
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? PartNumber { get; set; }
        public decimal? Qty { get; set; }
        public decimal? TotalWithTax { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }
        public decimal? BalanceQty { get; set; }
    }
}