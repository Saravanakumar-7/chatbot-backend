namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteSpecialTermsDto
    {
        public int Id { get; set; }
        public string? SpecialTerms { get; set; } 
    }
    public class QuoteSpecialTermsDtoPost
    {
        public string? SpecialTerms { get; set; }  
    }
    public class QuoteSpecialTermsDtoUpdate
    {
         public string? SpecialTerms { get; set; } 
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
