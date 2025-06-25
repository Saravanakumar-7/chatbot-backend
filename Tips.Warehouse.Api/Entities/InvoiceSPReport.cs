namespace Tips.Warehouse.Api.Entities
{
    public class InvoiceSPReport
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? DONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? leadid { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? SalesOrderNumber { get; set; }
        public int? RevisionNumber { get; set; }
        public DateTime? DODate { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? OrderType { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? issuedby { get; set; }
        public string? IssuedTo { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public string? DiscountType { get; set; }
        public string? Discount { get; set; }
        public decimal? TotalValueWithTax { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? UTGST { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? orderQTY { get; set; }
        public decimal? IssuedQty { get; set; }
        public decimal? InvoicedQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public string? SerialNumber { get; set; }
        public string? Remarks { get; set; }
    }
}
