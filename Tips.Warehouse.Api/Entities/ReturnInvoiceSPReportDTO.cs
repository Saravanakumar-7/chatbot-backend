namespace Tips.Warehouse.Api.Entities
{
    public class ReturnInvoiceSPReportDTO
    {
        public string? InvoiceNumber { get; set; }

        public string? DoNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }

        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? IssuedTo { get; set; }

    }
}
