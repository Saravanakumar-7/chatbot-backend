using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class POBreakDownForAvi
    {
        public int Id { get; set; }
        public string PONumber { get; set; }
        public string VendorId { get; set; }

        [Precision(13, 3)]
        public decimal TotalValue { get; set; }

        [Precision(13, 3)]
        public decimal PendingValue { get; set; }

        [Precision(13, 3)]
        public decimal AmountAgainstPO { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? NextPayment { get; set; }
        public string? ReferenceNO { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentTerms { get; set; }
        public string? OtherRemarks { get; set; }
        public int POCollectionTrackerForAviId { get; set; }
        public POCollectionTrackerForAvi? POCollectionTrackerForAvi { get; set; }
    }
}
