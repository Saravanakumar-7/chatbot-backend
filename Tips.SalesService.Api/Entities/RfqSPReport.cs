namespace Tips.SalesService.Api.Entities
{
    public class RfqSPReport
    {
        public string? LeadId { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? RfqNumber { get; set; }
        public int? RevisionNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
    }
}
