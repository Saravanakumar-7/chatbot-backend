namespace Tips.Production.Api.Entities
{
    public class MaterialIssueLocation
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? DistributingQty { get; set; }
        public int MaterialIssueItemId { get; set; }
        public MaterialIssueItem MaterialIssueItem { get; set; }
    }
}
