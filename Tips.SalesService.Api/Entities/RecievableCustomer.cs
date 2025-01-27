namespace Tips.SalesService.Api.Entities
{
    public class RecievableCustomer
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? TypeOfSolution { get; set; }
        public decimal? SOValue { get; set; }
        public decimal? DOValue { get; set; }
        public decimal? SOBalance { get; set; }
        public decimal? AdvanceReceived { get; set; }
        public decimal? BalanceAvailable { get; set; }
        public decimal? InvoiceValue { get; set; }
        public decimal? NetBalanceAvailable { get; set; }
        public decimal? PendingInvoiceValueAgainstDO { get; set; }
        public decimal? PendingAmountAganistSo { get; set; }
        public decimal? PendingInvoiceValueAgainstSO { get; set; }
    }
}
