namespace Tips.SalesService.Api.Entities
{
    public class CollectionTracker
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalSumOfSOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SOBreakDown>? SOBreakDown { get; set; }
    }
}
