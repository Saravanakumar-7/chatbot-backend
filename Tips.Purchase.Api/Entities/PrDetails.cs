namespace Tips.Purchase.Api.Entities
{
    public class PrDetails
    {
        public int Id { get; set; }
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
        public bool ToClosePR { get; set; } = false;
        public int POItemDetailId { get; set; }
        public PoItem? POItemDetail { get; set; } 
    }
}
