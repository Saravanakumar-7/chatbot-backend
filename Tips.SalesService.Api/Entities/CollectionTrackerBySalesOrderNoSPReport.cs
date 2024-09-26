namespace Tips.SalesService.Api.Entities
{
    public class CollectionTrackerBySalesOrderNoSPReport
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public decimal? sum { get; set; }
        public decimal? pending { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
