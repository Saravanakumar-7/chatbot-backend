namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnOpenDeliveryOrderSPReportDTO
    {
        public string? ODONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? IssuedTo { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ODOType { get; set; }

    }
    public class ReturnOpenDeliveryOrderSPReportForTransDTO
    {
        public string? ODONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? IssuedTo { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ODOType { get; set; }
        public string? ProjectNumber { get; set; }

    }
}
