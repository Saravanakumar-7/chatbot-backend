namespace Tips.SalesService.Api.Entities
{
    public class CollectionTrackerByCustomerIdSPReport
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal? sumofvalue { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
