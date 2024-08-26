namespace Entities
{
    public class WeightedAvgRate
    {
       public int Id { get; set; }  
        public string? Itemnumber { get; set; }
        public decimal totalbalance_qty { get; set; }
        public decimal Avg_cost { get; set; }
        public DateTime? update_date { get; set; }
        public TimeSpan? updated_time { get; set; }

    }
}
