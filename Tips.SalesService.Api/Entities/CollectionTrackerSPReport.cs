namespace Tips.SalesService.Api.Entities
{
    public class CollectionTrackerSPReport
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Remarks { get; set; }
        public decimal? TotalSumOfSOAmount { get; set; }
        public decimal? AmountRecieved { get; set; }
        public decimal? AlreadyRecieved { get; set; }
        public string? PaymentMode { get; set; }
        public string? PaymentRefNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal? AlreadyRecievedWithPercentage { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? PendingValue { get; set; }
        public decimal? AmountAgainstSO { get; set; }
    }
}
