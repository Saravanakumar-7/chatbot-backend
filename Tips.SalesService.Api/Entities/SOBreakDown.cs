namespace Tips.SalesService.Api.Entities
{
    public class SOBreakDown
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string CustomerId { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstSO { get; set; }
        public int CollectionTrackerId { get; set; }
        public CollectionTracker? CollectionTracker { get; set; }
    }
}
