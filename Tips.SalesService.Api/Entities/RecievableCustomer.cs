namespace Tips.SalesService.Api.Entities
{
    public class RecievableCustomer
    {
        public string? SalesOrderNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? CustomerId { get; set; }
        public decimal? Sovalue { get; set; }
        public int? sbid { get; set; }
        public decimal? Dovalue { get; set; }
        public decimal? Invoicedvalue { get; set; }
        public decimal? SObalance { get; set; }
        public decimal? Advancereceived { get; set; }
        public decimal? Balanceavailable { get; set; }
        public decimal? pendinginvoicevalueagainstDO { get; set; }
        public decimal? pendingamountagainstSO { get; set; }
        public decimal? pendinginvoiceagainstSO { get; set; }
    }
}
