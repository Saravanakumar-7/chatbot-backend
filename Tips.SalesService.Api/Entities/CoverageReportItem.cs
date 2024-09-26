namespace Tips.SalesService.Api.Entities
{
    internal class CoverageReportItem
    {
        public string Id { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public decimal TotalRequiredQty { get; set; }
        public decimal TotalStockAvailable { get; set; }
        public decimal OpenPOQty { get; set; }
        public decimal BalanceQtyToOrder { get; set; }
        public string Status { get; set; }
    }
}