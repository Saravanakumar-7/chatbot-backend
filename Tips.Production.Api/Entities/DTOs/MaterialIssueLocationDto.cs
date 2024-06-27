namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialIssueLocationDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? DistributingQty { get; set; }
    }
    public class MaterialIssueLocationPostDto
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? DistributingQty { get; set; }
    }
    public class MaterialIssueLocationUpdateDto
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? DistributingQty { get; set; }
    }
}
