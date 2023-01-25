using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? QuoteRef { get; set; }

        [Precision(18,3)]
        public decimal? TotalAmount { get; set; }
        public bool? IsTheseRequiredToBePrintedInQuote { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }
        public string? PaymentTerms { get; set; }
        public string? QuoteType { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }

        [Precision(13, 3)]
        public decimal? GeneralDiscount { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<QuoteGeneral>? QuoteGenerals { get; set; }
        public List<QuoteAdditionalCharges>? QuoteAdditionalCharges { get; set; }
        public List<QuoteRFQNotes>? QuoteRFQNotes { get; set; }
        public List<QuoteOtherTerms>? QuoteOtherTerms { get; set; }
        public List<QuoteSpecialTerms>? QuoteSpecialTerms { get; set; }
    }
}
