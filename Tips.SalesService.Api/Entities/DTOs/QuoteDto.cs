using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        public string? SalesPerson { get; set; }
        public OrderStatus QuoteStatus { get; set; }
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
        public bool IsShortClosed { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }

        [Precision(13, 3)]
        public decimal? GeneralDiscount { get; set; }
        public string? LeadTime { get; set; }
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
    public class QuoteforKeusDto
    {
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? RoomName { get; set; }
        public string? SalesPerson { get; set; }
        public OrderStatus QuoteStatus { get; set; }
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
        public bool IsShortClosed { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }

        [Precision(13, 3)]
        public decimal? GeneralDiscount { get; set; }
        public string? FirstEmailQuoteNo { get; set; }
        public DateTime? FirstEmailSentOn { get; set; }
        public decimal? FirstEmailQuoteRevNo { get; set; }
        public decimal? FirstEmailQuoteValue { get; set; }
        public decimal? QuoteUntaxedValue { get; set; }
        public DateTime? RevCreatedOnDate { get; set; }
        public string? LeadTime { get; set; }

        // public decimal? EmailSentValue { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        //public List<QuoteGeneralDto>? QuoteGeneralDtos { get; set; }
        //public List<QuoteAdditionalChargesDto>? QuoteAdditionalChargesDtos { get; set; }
        //public List<QuoteRFQNotesDto>? QuoteRFQNotesDtos { get; set; }
        //public List<QuoteOtherTermsDto>? QuoteOtherTermsDtos { get; set; }
        //public List<QuoteSpecialTermsDto>? QuoteSpecialTermsDtos { get; set; }
    }

    public class QuotePostDto
    {
        public string? LeadId { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? RoomName { get; set; }
        public string? SalesPerson { get; set; }

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
        public string? LeadTime { get; set; }

        [Precision(13, 3)]
        public decimal? GeneralDiscount { get; set; }
        public List<QuoteGeneralPostDto>? QuoteGeneralPostDtos { get; set; }
        public List<QuoteAdditionalChargesPostDto>? QuoteAdditionalChargesPostDtos { get; set; }
        public List<QuoteRFQNotesPostDto>? QuoteRFQNotesPostDtos { get; set; }
        public List<QuoteOtherTermsPostDto>? QuoteOtherTermsPostDtos { get; set; }
        public List<QuoteSpecialTermsPostDto>? QuoteSpecialTermsPostDtos { get; set; }
    }
    public class QuoteUpdateDto
    {
        public string? LeadId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? SalesPerson { get; set; }

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
        public string? LeadTime { get; set; }
        public string? Remarks { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
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
        public string? ImageURL { get; set; }

    }
    public class rfqEnggItemDetailsForQuoteDto
    {
        //public int? Id { get; set; }
        public string? LeadId { get; set; }

        public string? RfqNumber { get; set; }
        public string? CustomerName { get; set; }
        public decimal? Rev { get; set; }

        //public DateTime? DateOnLpCreation { get; set; }
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
        public string? PriceListName { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
        public string? ImageURL { get; set; }
    }
    // To allow short closed
    public class ShortClosedDto
    {
        public string? QuoteNumber { get; set; }
        public string? ShortClosedRemarks { get; set; }
    }


    public class Itemnumberimages
    {
        public List<Datum>? data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string? itemnumber { get; set; }
        public string? fileName { get; set; }
        public string? fileExtension { get; set; }
        public string? downloadUrl { get; set; }
    }
    public class Data1
    {
        public int id { get; set; }
        public string processType { get; set; }
        public string? template { get; set; }
        public string subject { get; set; }
    }

    public class EmailTemplateDto
    {
        public Data1 data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
    public class SalesEmailIDsDto
    {
        public Datum1[] data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datum1
    {
        public int id { get; set; }
        public string operation { get; set; }
        public string emailIds { get; set; }
        public string? host { get; set; }
        public int? port { get; set; }
        public string? password { get; set; }
    }
    public class QuoteEmailPostDto
    {
        public string SentTo { get; set; }
        public string? CusEmail { get; set; }
        //public string jasperfileUrl { get; set; }
        public int Quoteid { get; set; }
        public string WhatsAppPhoneNos { get; set; }
    }
    public class QuoteSPResportParamDTO
    {
        public string? CustomerId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? QuotationVersionNo { get; set; }
    }
    public class SoSummaryQuotePostDto
    {
        public string? FirstQuotenumber { get; set; }
        public string? SOLatestSalesOrderSentNumber { get; set; }
        public string? Leadid { get; set; }
        public string? CustomerName { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
    }
    public class SoSummaryQuotationDto
    {
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? FirstQuoteNumber { get; set; }
        public decimal? FirstQuoteRevisionNumber { get; set; }
        public DateTime? FirstQuoteCreatedOn { get; set; }
        public decimal? FirstQuoteTotalFinalAmount { get; set; }
        public string? FirstQuoteSentNumber { get; set; }
        public decimal? FirstQuoteSentRevisionNumber { get; set; }
        public DateTime? FirstQuoteSentCreatedOn { get; set; }
        public decimal? FirstQuoteSentGeneralDiscount { get; set; }
        public string? FirstQuoteSentGeneralDiscountType { get; set; }
        public decimal? FirstQuoteSentTaxedValue { get; set; }
        public decimal? FirstQuoteSentUntaxedValue { get; set; }
        public DateTime? FirstQuoteEmailSentOn { get; set; }
        public string? LatestQuoteSentNumber { get; set; }
        public decimal? LatestQuoteSentRevisionNumber { get; set; }
        public DateTime? LatestQuoteSentCreatedOn { get; set; }
        public decimal? LatestQuoteSentGeneralDiscount { get; set; }
        public string? LatestQuoteSentGeneralDiscountType { get; set; }
        public decimal? LatestQuoteSentTaxedValue { get; set; }
        public decimal? LatestQuoteSentUntaxedValue { get; set; }
        public string? FirstSalesOrderSentNumber { get; set; }
        public int? FirstSOSentRevisionNumber { get; set; }
        public DateTime? FirstSOSentCreatedOn { get; set; }
        public decimal? FirstSOSentDiscount { get; set; }
        public string? FirstSOSentDiscountType { get; set; }
        public decimal? FirstSOSentTaxedValue { get; set; }
        public decimal? FirstSOSentUntaxedValue { get; set; }
        public string? LatestSalesOrderSentNumber { get; set; }
        public int? LatestSOSentRevisionNumber { get; set; }
        public DateTime? LatestSOSentCreatedOn { get; set; }
        public decimal? LatestSOSentDiscount { get; set; }
        public string? LatestSOSentDiscountType { get; set; }
        public decimal? LatestSOSentTaxedValue { get; set; }
        public decimal? LatestSOSentUntaxedValue { get; set; }
    }

    public class QuoteRevNoSPReportParamDTO
    {
        public string? LeadId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? ItemNumber { get; set; }
    }
    public class QuoteRevNoSPReportParam
    {
        public string? Leadid { get; set; }
        public string? CustomerName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? QuoteNumber { get; set; }
        public decimal? RevisionNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public DateTime? Created_date { get; set; }
        public string? CreatedBy { get; set; }
        public string? KPN { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Discountper { get; set; }
        public decimal? Discount_value { get; set; }
        public decimal? NetQuote_PostDiscount { get; set; }
        public decimal? TotalAdditionalCharges { get; set; }
        public decimal? TotalFinalAmount { get; set; }
    }


    public class QuoteItemMasterDetails
    {
        public Datas data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datas
    {
        public int id { get; set; }
        public string itemNumber { get; set; }
        public string? description { get; set; }
        public bool isActive { get; set; }
        //public bool isObsolete { get; set; }
        public int itemType { get; set; }
        public string? uom { get; set; }
        //public string commodity { get; set; }
        //public string hsn { get; set; }
        //public string materialGroup { get; set; }
        //public DateTime? validFrom { get; set; }
        //public DateTime? validTo { get; set; }
        //public string purchaseGroup { get; set; }
        //public string department { get; set; }
        //public string customerPartReference { get; set; }
        //public bool isPRRequired { get; set; }
        //public string poMaterialType { get; set; }
        //public bool openGrin { get; set; }
        //public bool isCustomerSuppliedItem { get; set; }
        //public string drawingNo { get; set; }
        //public string docRet { get; set; }
        //public string revNo { get; set; }
        //public bool isCocRequired { get; set; }
        //public bool isRohsItem { get; set; }
        //public bool isShelfLife { get; set; }
        //public bool isReachItem { get; set; }
        //public int imageUpload { get; set; }
        //public string fileUpload { get; set; }
        //public decimal netWeight { get; set; }
        //public string netUom { get; set; }
        //public decimal grossWeight { get; set; }
        //public string grossUom { get; set; }
        //public decimal volume { get; set; }
        //public string volumeUom { get; set; }
        //public decimal size { get; set; }
        //public string footPrint { get; set; }
        public decimal? min { get; set; }
        public decimal? max { get; set; }
        //public string leadTime { get; set; }
        //public string reorder { get; set; }
        //public string twoBin { get; set; }
        //public bool kanban { get; set; }
        //public bool isEsd { get; set; }
        //public bool isFifo { get; set; }
        //public bool isLifo { get; set; }
        //public bool isCycleCount { get; set; }
        //public bool isHazardousMaterial { get; set; }
        //public string expiry { get; set; }
        //public string inspectionInterval { get; set; }
        //public string specialInstructions { get; set; }
        //public string shippingInstruction { get; set; }
        //public bool isIQCRequired { get; set; }
        //public int grProcessing { get; set; }
        //public string batchSize { get; set; }
        //public string costCenter { get; set; }
        //public decimal stdCost { get; set; }
        //public string costingMethod { get; set; }
        //public bool valuation { get; set; }
        //public bool depreciation { get; set; }
        //public bool pfo { get; set; }
        //public string unit { get; set; }
        //public string remarksToVendor { get; set; }
        //public string createdBy { get; set; }
        //public DateTime? createdOn { get; set; }
        //public string lastModifiedBy { get; set; }
        //public DateTime? lastModifiedOn { get; set; }
        public Itemmasteralternate[] itemmasterAlternate { get; set; }
        //public Itemmasterwarehouse[] itemMasterWarehouse { get; set; }
        //public Itemmasterapprovedvendor[] itemMasterApprovedVendor { get; set; }
        //public Itemmasterrouting[] itemMasterRouting { get; set; }
    }

    public class Itemmasteralternate
    {
        public int id { get; set; }
        public string manufacturerPartNo { get; set; }
        public string? manufacturer { get; set; }
        public bool? alternateSource { get; set; }
        public bool? isDefault { get; set; }
        //public string createdBy { get; set; }
        //public DateTime? createdOn { get; set; }
        //public string lastModifiedBy { get; set; }
        //public DateTime? lastModifiedOn { get; set; }
    }
    public class QuoteNoDto
    {
        public string? QuoteNumber { get; set; }
        public List<decimal?> RevisionNumber { get; set; }
    }
    public class QuoteNumberDto
    {
        public int Id {  get; set; }
        public string? QuoteNumber { get; set; }
    }
    public class QuoteSpReportDto
    {
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? RfqNumber { get; set; }
    }

    public class WhatsAppMessagePayload
    {
        [JsonProperty("recipient_type")]
        public string RecipientType { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("template")]
        public Template Template { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    public class Template
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("components")]
        public List<Component> Components { get; set; }
    }

    public class Language
    {
        [JsonProperty("policy")]
        public string Policy { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class Component
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("parameters")]
        public List<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("document")]
        public Document? Document { get; set; }
        [JsonProperty("text")]
        public string? Text { get; set; } = null;
    }

    public class Document
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("messageId")]
        public string MessageId { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }
    }

    public class Media
    {
        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        //[JsonProperty("content")]
        //public string Content { get; set; }
    }


    public class WhatsAppCreateTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("not-before-policy")]
        public int NotBeforePolicy { get; set; }

        [JsonProperty("session_state")]
        public string SessionState { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
    public class MsisdnListRequest
    {
        [JsonProperty("msisdnList")]
        public List<string> MsisdnList { get; set; }
    }
    public class QuoteEmailMessageSuccessMessage
    {
        public string QuoteNumber { get; set; }
        public decimal RevisionNumber { get; set; }
    }
}
