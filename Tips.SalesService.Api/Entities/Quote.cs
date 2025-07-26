using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? RoomName { get; set; }
        public OrderStatus QuoteStatus { get; set; }
        public string? QuoteRef { get; set; }
        public string? generalDiscountType { get; set; }        

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
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }
        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }

        [Precision(13, 3)]
        public decimal? TotalAmountWithInstallationCharges { get; set; }

        public string? ReasonForModification { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }

        [Precision(13, 3)]
        public decimal? GeneralDiscount { get; set; }
        
        public string? SalesPerson { get; set; }
       // public string? LeadTime { get; set; }
        public string Unit { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
         
        public bool IsShortClosed { get; set; } = false;
        public string? ShortClosedRemarks { get; set; }

        public List<QuoteGeneral>? QuoteGenerals { get; set; }
        public List<QuoteAdditionalCharges>? QuoteAdditionalCharges { get; set; }
        public List<QuoteRFQNotes>? QuoteRFQNotes { get; set; }
        public List<QuoteOtherTerms>? QuoteOtherTerms { get; set; }
        public List<QuoteSpecialTerms>? QuoteSpecialTerms { get; set; }
    }
}
