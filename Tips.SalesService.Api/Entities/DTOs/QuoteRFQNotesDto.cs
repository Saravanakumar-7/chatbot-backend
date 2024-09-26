namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteRFQNotesDto
    {
        public int Id { get; set; }
        public string? RFQNotes { get; set; } 
    }
    public class QuoteRFQNotesPostDto
    {
        public string? RFQNotes { get; set; } 
    }
    public class QuoteRFQNotesUpdateDto
    {
        public string? RFQNotes { get; set; } 
    }
}
