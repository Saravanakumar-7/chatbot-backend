namespace Tips.Grin.Api.Entities.DTOs
{
    public class WeightedAvgRateDto
    {
        public int Id { get; set; }
        public string? Itemnumber { get; set; }
        public int totalbalance_qty { get; set; }
        public int Avg_cost { get; set; }
        public DateTime? update_date { get; set; }
        public TimeSpan? updated_time { get; set; }
    }
}
