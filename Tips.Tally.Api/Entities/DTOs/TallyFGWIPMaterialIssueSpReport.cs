namespace Tips.Tally.Api.Entities.DTOs
{
    public class TallyFGWIPMaterialIssueSpReport
    {
        public string? Number { get; set; }
        public DateTime? Date { get; set; }
        public int? Id { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UOM { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string? OutLocation { get; set; }
        public string? InLocation { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
