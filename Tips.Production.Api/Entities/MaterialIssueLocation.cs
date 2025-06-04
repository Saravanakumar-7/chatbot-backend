namespace Tips.Production.Api.Entities
{
    public class MaterialIssueLocation
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? LocationStock { get; set; }
        public decimal? DistributingQty { get; set; }
        public int MaterialIssueItemId { get; set; }
        public DateTime? IssuedOn {  get; set; }
        public string? IssuedBy { get; set; }
        public MaterialIssueItem MaterialIssueItem { get; set; }
    }
}
