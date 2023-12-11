namespace Tips.Warehouse.Api.Entities
{
    public class InvoiceSPReport
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public string? DONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? RevisionNumber { get; set; }
        public DateTime? DODate { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? OrderType { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? AvaliableQty { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public string? SerialNumber { get; set; }
        public string? Remarks { get; set; }
    }
}
