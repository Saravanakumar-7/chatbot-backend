namespace Tips.Warehouse.Api.Entities
{
    public class CrossMarginSPReport
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? ItemNumber { get; set; }
        public string? OrderNumber { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? Avg_cost { get; set; }
        public string? Status { get; set; }
    }
}
