namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteOtherTermsDto
    {
        public int Id { get; set; }
        public string? OtherTerms { get; set; } 
    }
    public class QuoteOtherTermsPostDto
    {
        public string? OtherTerms { get; set; } 
    }
    public class QuoteOtherTermsUpdateDto
    {
        public string? OtherTerms { get; set; } 
    }
}
