using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class POCollectionTracker
    {
        public int Id { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }

        [Precision(13, 3)]
        public decimal TotalSumOfPOAmount { get; set; }

        [Precision(13, 3)]
        public decimal AmountRecieved { get; set; }

        [Precision(13, 3)]
        public decimal AlreadyRecieved { get; set; }

        [Precision(13, 3)]
        public decimal AlreadyRecievedWithPercentage { get; set; }
        public string? Remarks { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDown>? POBreakDown { get; set; }
    }
}
