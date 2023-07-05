namespace Tips.Purchase.Api.Entities
{
    public class POBreakDown
    {
        public int Id { get; set; }
        public string PONumber { get; set; }
        public string VendorId { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstPO { get; set; }
        public int POCollectionTrackerId { get; set; }
        public POCollectionTracker? POCollectionTracker { get; set; }
    }
}
