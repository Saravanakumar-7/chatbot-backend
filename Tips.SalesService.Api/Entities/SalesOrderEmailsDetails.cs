namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderEmailsDetails
    {
        public int Id { get; set; }
        public int? SalesOrderId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? RevisionNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public decimal? SalesOrderValue { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? SentBy { get; set; }
        public DateTime? SentOn { get; set; }
        public string? SentTo { get; set; }
        public string? CustomerEmailId { get; set; }
    }
}
