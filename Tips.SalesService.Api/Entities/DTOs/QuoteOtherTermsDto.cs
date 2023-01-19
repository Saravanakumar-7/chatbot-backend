namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteOtherTermsDto
    {
        public int Id { get; set; }
        public string? OtherTerms { get; set; } 
    }
    public class QuoteOtherTermsDtoPost
    {
        public string? OtherTerms { get; set; } 
    }
    public class QuoteOtherTermsDtoUpdate
    {
        public string? OtherTerms { get; set; } 
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
