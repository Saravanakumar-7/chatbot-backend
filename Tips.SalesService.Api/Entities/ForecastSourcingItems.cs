namespace Tips.SalesService.Api.Entities
{
    public class ForecastSourcingItems
    {
       
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public int? QuantityReq { get; set; }
        public int? Count { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int ForeCastSourcingId { get; set; }
        public ForecastSourcing? ForecastSourcing { get; set; }

        public List<ForecastSourcingVendor>? forecastSourcingVendors { get; set; }
    }
}
