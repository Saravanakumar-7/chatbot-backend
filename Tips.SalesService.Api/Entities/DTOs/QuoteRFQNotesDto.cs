namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteRFQNotesDto
    {
        public int Id { get; set; }
        public string? RFQNotes { get; set; } 
    }
    public class QuoteRFQNotesDtoPost
    {
        public string? RFQNotes { get; set; } 
    }
    public class QuoteRFQNotesDtoUpdate
    {
        public string? RFQNotes { get; set; } 
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
