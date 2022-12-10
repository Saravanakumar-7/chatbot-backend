namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteOtherTermsDto
    {
        public int Id { get; set; }
        public string? OtherTerms { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class QuoteOtherTermsDtoPost
    {
        public string? OtherTerms { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class QuoteOtherTermsDtoUpdate
    {
        public int Id { get; set; }
        public string? OtherTerms { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
