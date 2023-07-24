using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class POBreakDown
    {
        public int Id { get; set; }
        public string PONumber { get; set; }
        public string VendorId { get; set; }

        [Precision(13,3)]
        public decimal TotalValue { get; set; }

        [Precision(13, 3)]
        public decimal PendingValue { get; set; }

        [Precision(13, 3)]
        public decimal AmountAgainstPO { get; set; }
        public int POCollectionTrackerId { get; set; }
        public POCollectionTracker? POCollectionTracker { get; set; }
    }
}
