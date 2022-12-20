namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteDto
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
        public List<QuoteGeneralDto>? quoteGeneralDtos { get; set; }
        public List<QuoteAdditionalChargesDto>? quoteAdditionalChargesDtos { get; set; }
        public List<QuoteRFQNotesDto>? quoteRFQNotesDtos { get; set; }
        public List<QuoteOtherTermsDto>? quoteOtherTermsDtos { get; set; }
        public List<QuoteSpecialTermsDto>? quoteSpecialTermsDtos { get; set; }
    }
    public class QuoteDtoPost
    {
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
        public List<QuoteGeneralDtoPost>? quoteGeneralDtoPost { get; set; }
        public List<QuoteAdditionalChargesDtoPost>? quoteAdditionalChargesDtoPost { get; set; }
        public List<QuoteRFQNotesDtoPost>? quoteRFQNotesDtoPost { get; set; }
        public List<QuoteOtherTermsDtoPost>? quoteOtherTermsDtoPost { get; set; }
        public List<QuoteSpecialTermsDtoPost>? quoteSpecialTermsDtoPost {get; set; } 
    }
    public class QuoteDtoUpdate
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
        public List<QuoteGeneralDtoUpdate>? quoteGeneralDtoUpdate { get; set; }
        public List<QuoteAdditionalChargesDtoUpdate>? quoteAdditionalChargesDtoUpdate { get; set; }
        public List<QuoteRFQNotesDtoUpdate>? quoteRFQNotesDtoUpdate { get; set; }
        public List<QuoteOtherTermsDtoUpdate>? quoteOtherTermsDtoUpdate { get; set; }
        public List<QuoteSpecialTermsDtoUpdate>? quoteSpecialTermsDtoUpdate { get; set; }
    }
}
