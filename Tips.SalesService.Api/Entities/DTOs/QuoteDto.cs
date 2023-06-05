using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteDto
    {
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? RoomName { get; set; }
        public string? generalDiscountType { get; set; }
        public string? QuoteRef { get; set; }

        [Precision(18, 3)]
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
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<QuoteGeneralDto>? QuoteGeneralDtos { get; set; }
        public List<QuoteAdditionalChargesDto>? QuoteAdditionalChargesDtos { get; set; }
        public List<QuoteRFQNotesDto>? QuoteRFQNotesDtos { get; set; }
        public List<QuoteOtherTermsDto>? QuoteOtherTermsDtos { get; set; }
        public List<QuoteSpecialTermsDto>? QuoteSpecialTermsDtos { get; set; }
    }
    public class QuotePostDto
    {
        public string? LeadId { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? RoomName { get; set; }

        public string? QuoteRef { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        public string? generalDiscountType { get; set; }
        public bool? IsTheseRequiredToBePrintedInQuote { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal? TotalFinalAmount { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }
        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }

        [Precision(13, 3)]
        public decimal? TotalAmountWithInstallationCharges { get; set; }


        [Precision(13, 3)]
        public decimal? GeneralDiscount { get; set; } 
        public List<QuoteGeneralPostDto>? QuoteGeneralPostDtos { get; set; }
        public List<QuoteAdditionalChargesPostDto>? QuoteAdditionalChargesPostDtos { get; set; }
        public List<QuoteRFQNotesPostDto>? QuoteRFQNotesPostDtos { get; set; }
        public List<QuoteOtherTermsPostDto>? QuoteOtherTermsPostDtos { get; set; }
        public List<QuoteSpecialTermsPostDto>? QuoteSpecialTermsPostDtos {get; set; } 
    }
    public class QuoteUpdateDto
    {
        public string? LeadId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }

        public string? CustomerId { get; set; }
        public string? RoomName { get; set; }
        public string? ReasonForModification { get; set; }
 
        public string? QuoteRef { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        public string? generalDiscountType { get; set; }
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

        [Precision(13, 3)]
        public decimal? GeneralDiscount { get; set; }
        public string? Remarks { get; set; }

        public List<QuoteGeneralUpdateDto>? QuoteGeneralUpdateDtos { get; set; }
        public List<QuoteAdditionalChargesUpdateDto>? QuoteAdditionalChargesUpdateDtos { get; set; }
        public List<QuoteRFQNotesUpdateDto>? QuoteRFQNotesUpdateDtos { get; set; }
        public List<QuoteOtherTermsUpdateDto>? QuoteOtherTermsUpdateDtos { get; set; }
        public List<QuoteSpecialTermsUpdateDto>? QuoteSpecialTermsUpdateDtos { get; set; }
    }
    public class CsItemDetailsForQuoteDto
    {
        public string? LeadId { get; set; } 
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? RoomName { get; set; }
        public string? CustomerId { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? PriceListName { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; } 
        public decimal? LeastCostPlus { get; set; }

        [Precision(18, 3)]
        public decimal? LeastCostminus { get; set; }


        [Precision(18, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountMinus { get; set; }

        [Precision(18, 3)]
        public decimal? Markup { get; set; }

        public string? CustomFields { get; set; }

        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
        public DateTime? CreatedOn { get; set; }


    }
    public class rfqEnggItemDetailsForQuoteDto
    {
        public int? Id { get; set; }
        public string? RfqNumber { get; set; }
        public string? CustomerName { get; set; }
        public decimal? Rev { get; set; }
        public DateTime? DateOnLpCreation { get; set; }
        public string? CustomerItemNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomFields { get; set; }

        public string Description { get; set; }
        public bool ReleaseStatus { get; set; } = false;
        [Precision(18, 3)]
        public decimal? Qty { get; set; }
        public string? UOC { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCost { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostPlus { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostminus { get; set; }

        public decimal? CostingBomVersionNo { get; set; }

        [Precision(13, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(13, 3)]
        public decimal? DiscountMinus { get; set; }

        [Precision(18, 3)]
        public decimal? Markup { get; set; }
        public string? PriceList { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
    }
}
