namespace Tips.SalesService.Api.Entities.DTOs
{
    public class CollectionTrackerDto
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
        public List<SOBreakDownDto>? SOBreakDown { get; set; }
    }
    public class CollectionTrackerPostDto
    {
        public string CustomerName { get; set; }
        public decimal TotalSumOfSOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public List<SOBreakDownPostDto>? SOBreakDown { get; set; }

    }
    public class CollectionTrackerUpdateDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalSumOfSOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string Unit { get; set; }
        public List<SOBreakDownUpdateDto>? SOBreakDown { get; set; }

    }
}
