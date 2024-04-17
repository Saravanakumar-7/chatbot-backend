namespace Tips.SalesService.Api.Entities
{
    public class SOMonthlyConsumptionSPReport
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal? DispatchQty { get; set; }
    }
}
