namespace Tips.Warehouse.Api.Entities
{
    public class DoVsInvoiceSpReport
    {
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? BTONumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? Description { get; set; }
        public decimal? Discount { get; set; }
        public decimal? DOOrderQty { get; set; }
        public decimal? InitialDispatchQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? DOTotalValue { get; set; }
        public decimal? DOReturnQty { get; set; }
        public decimal? ReturnTotalValue { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal? InvoicedQty { get; set; }
        public decimal? BalanceInvoicedQty { get; set; }
        public decimal? ReturnInvoicedQty { get; set; }
        public decimal? InvoiceTotalValue { get; set; }
        public decimal? InvoiceReturnTotalValue { get; set; }
    }
    public class DoVsInvoiceInputParamDto
    {
        public string? SalesOrderNumber { get; set; }
        public string? DONumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerId { get; set; }

    }
}
