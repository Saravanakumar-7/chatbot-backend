using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class QuoteRFQNotes
    {
        [Key]
        public int Id { get; set; }
        public string? RFQNotes { get; set; } 
        public int QuoteId { get; set; }
        public Quote? Quote { get; set; }
    }
}
