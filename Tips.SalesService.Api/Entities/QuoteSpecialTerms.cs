namespace Tips.SalesService.Api.Entities
{
    public class QuoteSpecialTerms
    {
        public int Id { get; set; }
        public string? SpecialTerms { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int QuoteId { get; set; }
        public Quote? Quote { get; set; }
    }
}
