namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnDOSPReportDTO
    {
        public string? DoNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
    }
}
