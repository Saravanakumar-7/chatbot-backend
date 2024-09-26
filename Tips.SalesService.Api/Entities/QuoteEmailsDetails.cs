namespace Tips.SalesService.Api.Entities
{
    public class QuoteEmailsDetails
    {
        public int Id { get; set; }
        public string QuoteNumber { get; set; }
        public int QuoteId { get; set; }
        public decimal? RevisionNumber { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public decimal? QuoteValue { get; set; }
        public string? TypeOfSolution { get; set; }
        public string SentBy { get; set; }
        public DateTime SentOn { get; set;}
        public string SentTo { get; set; }
        public string? CustomerEmailId { get; set; }
        public string WhatsAppPhoneNos { get; set; }
    }
}
