using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

namespace Tips.SalesService.Api.Entities
{
    public class Quote
    {
        public int Id { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? QuoteRef { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsTheseRequiredToBePrintedInQuote { get; set; }
        public decimal TotalAdditionalCharges { get; set; }
        public decimal SpecialDiscountType { get; set; }
        public decimal SpecialDiscountAmount { get; set; }
        public decimal TotalFinalAmount { get; set; }
        public string? PaymentTerms { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<QuoteGeneral>? quoteGenerals { get; set; }
        public List<QuoteAdditionalCharges>? quoteAdditionalCharges { get; set; }
        public List<QuoteRFQNotes>? quoteRFQNotes { get; set; }
        public List<QuoteOtherTerms>? quoteOtherTerms { get; set; }
        public List<QuoteSpecialTerms>? quoteSpecialTerms { get; set; }
    }
}
