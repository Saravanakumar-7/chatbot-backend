namespace Tips.Purchase.Api.Entities.Dto
{
    public class POBreakDownForAviDto
    {
        public int Id { get; set; }
        public string VendorId { get; set; }

        public string PONumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstPO { get; set; }
        public DateTime? Date { get; set; }
        public string? ReferenceNO { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentTerms { get; set; }
        public string? OtherRemarks { get; set; }
    }
    public class POBreakDownForAviPostDto
    {
        public string VendorId { get; set; }

        public string PONumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstPO { get; set; }
        public DateTime? Date { get; set; }
        public string? ReferenceNO { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentTerms { get; set; }
        public string? OtherRemarks { get; set; }
    }
    public class POBreakDownForAviUpdateDto
    {
        public int Id { get; set; }
        public string VendorId { get; set; }

        public string PONumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstPO { get; set; }
        public DateTime? Date { get; set; }
        public string? ReferenceNO { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentTerms { get; set; }
        public string? OtherRemarks { get; set; }
    }
}
