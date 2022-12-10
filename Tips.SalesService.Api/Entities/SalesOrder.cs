namespace Tips.SalesService.Api.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string? RevNo { get; set; }

        //PO Details
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public DateTime ReceivedDate { get; set; }

        //Billing&Shipping
        public string? BillTo { get; set; }
        public int BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int ShipToId { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Remarks { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<SalesOrderItems>? salesOrdersItems { get; set; }


    }
}
