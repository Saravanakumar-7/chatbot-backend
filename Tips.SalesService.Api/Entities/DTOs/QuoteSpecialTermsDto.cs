namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteSpecialTermsDto
    {
        public int Id { get; set; }
        public string? SpecialTerms { get; set; } 
    }
    public class QuoteSpecialTermsPostDto
    {
        public string? SpecialTerms { get; set; }  
    }
    public class QuoteSpecialTermsUpdateDto
    {
        public string? SpecialTerms { get; set; } 
      
    }
}
