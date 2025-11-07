namespace Tips.Tally.Api.Entities.DTOs
{
    public class TallyStockItemSPReport
    {   public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Group { get; set; }
        public int? Category { get; set; }
        public string? Uom { get; set; }
        public string? Hsn { get; set; }
        public string? GSTRate { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
