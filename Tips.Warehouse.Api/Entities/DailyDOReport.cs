namespace Tips.Warehouse.Api.Entities
{
    public class DailyDOReport
    {
        public string? LeadId { get; set; }
        public string? SONumber { get; set; }
        public string? DOnumber { get; set; }
        public DateTime? DODate { get; set; }
        public string? DispatchKPN { get; set; }
        public string? Description { get; set; }
        public decimal? Qnty { get; set; }


    }
}