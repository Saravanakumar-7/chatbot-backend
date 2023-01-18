using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class QuoteOtherTerms
    {
        [Key]
        public int Id { get; set; }
        public string? OtherTerms { get; set; } 
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int QuoteId { get; set; }
        public Quote? Quote { get; set; }
    }
}
