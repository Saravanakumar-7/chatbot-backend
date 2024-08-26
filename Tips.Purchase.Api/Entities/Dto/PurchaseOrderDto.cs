using System.ComponentModel;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int? RevisionNumber { get; set; }
        public int? BillToId { get; set; }
        public int? ShipToId { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public string? CompanyAliasName { get; set; }
        public bool PoConfirmationStatus { get; set; }
        public string? Transports { get; set; }
        public string? Other { get; set; }
        public PoStatus PoStatus { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string? QuotationReferenceNumber { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? VendorAddress { get; set; }

        //Billing&ShippingDetails
        public string? DeliveryTerms { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }
        //public List<DocumentUploadPostDto>? POFiles { get; set; }
        public string? POFiles { get; set; }
        public bool TallyStatus { get; set; } = false;
        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }
        public bool IsDeleted { get; set; } = false;

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public decimal TotalAmount { get; set; }
        public bool POApprovalI { get; set; }
        public DateTime POApprovedIDate { get; set; }
        public string? POApprovedIBy { get; set; }
        public bool POApprovalII { get; set; }
        public DateTime POApprovedIIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public bool POApprovalIII { get; set; }
        public string? POApprovedIIIBy { get; set; }
        public DateTime? POApprovedIIIDate { get; set; }
        public bool POApprovalIV { get; set; }
        public string? POApprovedIVBy { get; set; }
        public DateTime? POApprovedIVDate { get; set; }
        public int? ApprovalCount { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoItemsDto>? POItems { get; set; }
        public List<PoIncoTermDto>? POIncoTerms { get; set; }

    }

    public class PurchaseOrderPostDto
    {
        public DateTime PODate { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public int? BillToId { get; set; }
        public int? ShipToId { get; set; }
        public string? POFiles { get; set; }

        //public string? POFiles { get; set; }
        public string? CompanyAliasName { get; set; }
        public string? Transports { get; set; }
        public string? Other { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string? QuotationReferenceNumber { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? VendorAddress { get; set; }
        public bool TallyStatus { get; set; } = false;
        public string? DeliveryTerms { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }
        //public string? Transport { get; set; }
        public string? Others { get; set; }
        public decimal TotalAmount { get; set; }
        public int? ApprovalCount { get; set; }

        public List<PoItemsPostDto>? POItems { get; set; }
        public List<PoIncoTermPostDto>? POIncoTerms { get; set; }

    }
    public class PurchaseOrderUpdateDto
    {

        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public string? Transports { get; set; }
        public string? Other { get; set; }
        public string? POFiles { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public int? BillToId { get; set; }
        public int? ShipToId { get; set; }
        public string? VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string? QuotationReferenceNumber { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? VendorAddress { get; set; }
        public string? CompanyAliasName { get; set; }
        public bool TallyStatus { get; set; } = false;
        public string? DeliveryTerms { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }
        public decimal TotalAmount { get; set; }
        public int? ApprovalCount { get; set; }
        public PoStatus PoStatus { get; set; }
        public string? Unit { get; set; }
        //public string? LastModifiedBy { get; set; }
        //public DateTime? LastModifiedOn { get; set; }

        public List<PoItemsUpdateDto>? POItems { get; set; }
        public List<PoIncoTermUpdateDto>? POIncoTerms { get; set; }

    }
    public class PurchaseOrderForShortCloseDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int? RevisionNumber { get; set; }
        public int? BillToId { get; set; }
        public int? ShipToId { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public string? CompanyAliasName { get; set; }
        public bool PoConfirmationStatus { get; set; }
        public string? Transports { get; set; }
        public string? Other { get; set; }
        public PoStatus PoStatus { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string? QuotationReferenceNumber { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? VendorAddress { get; set; }

        //Billing&ShippingDetails
        public string? DeliveryTerms { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }
        //public List<DocumentUploadPostDto>? POFiles { get; set; }
        public string? POFiles { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }
        public bool IsDeleted { get; set; } = false;

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public decimal TotalAmount { get; set; }
        public bool POApprovalI { get; set; }
        public DateTime POApprovedIDate { get; set; }
        public string? POApprovedIBy { get; set; }
        public bool POApprovalII { get; set; }
        public DateTime POApprovedIIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public bool POApprovalIII { get; set; }
        public string? POApprovedIIIBy { get; set; }
        public DateTime? POApprovedIIIDate { get; set; }
        public bool POApprovalIV { get; set; }
        public string? POApprovedIVBy { get; set; }
        public DateTime? POApprovedIVDate { get; set; }
        public int? ApprovalCount { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool TallyStatus { get; set; } = false;
        public List<PoItemsShortCloseDto>? POItems { get; set; }
        public List<PoIncoTermUpdateDto>? POIncoTerms { get; set; }

    }
    public class PurchaseOrderIdNameListDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int? RevisionNumber { get; set; }
        public int? BillToId { get; set; }
        public int? ShipToId { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public string? CompanyAliasName { get; set; }
        public bool PoConfirmationStatus { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string? QuotationReferenceNumber { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? VendorAddress { get; set; }

        //Billing&ShippingDetails
        public string? DeliveryTerms { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }
        //public List<DocumentUploadPostDto>? POFiles { get; set; }
        public string? POFiles { get; set; }
        public bool TallyStatus { get; set; } = false;
        //Terms
        public string? RetentionPeriod { get; set; }
        public string? Transport { get; set; }
        public string? Others { get; set; }
        public string? SpecialTermsAndConditions { get; set; }
        public bool IsDeleted { get; set; } = false;

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public decimal TotalAmount { get; set; }
        public bool POApprovalI { get; set; }
        public DateTime POApprovedIDate { get; set; }
        public string? POApprovedIBy { get; set; }
        public bool POApprovalII { get; set; }
        public DateTime POApprovedIIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public bool POApprovalIII { get; set; }
        public string? POApprovedIIIBy { get; set; }
        public DateTime? POApprovedIIIDate { get; set; }
        public bool POApprovalIV { get; set; }
        public string? POApprovedIVBy { get; set; }
        public DateTime? POApprovedIVDate { get; set; }
        public int? ApprovalCount { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class PurchaseOrderSearchDto
    {
        public List<string>? PONumber { get; set; }
        public List<string>? ProcurementType { get; set; }
        public List<string>? ShippingMode { get; set; }
        public List<string>? VendorName { get; set; }
        public List<Status>? PoStatus { get; set; }

    }
    public class PurchaseOrderRevNoListDto
    {
        public int? RevisionNumber { get; set; }
    }
    public class PRNoandQtyListDto
    {
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
        public int? RevisionNumber { get; set; }

        public List<PRItemsDocumentUpload>? DocumentNames { get; set; }

    }

    public class PRNoandQtyListDocsDto
    {
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
        public int? RevisionNumber { get; set; }

        public List<PRItemsDocumentUploadDocsDto> DocumentNames { get; set; }

    }

    public class PurchaseOrderReportDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public string? CompanyAliasName { get; set; }
        public string? Transports { get; set; }
        public string? Other { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string? QuotationReferenceNumber { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? VendorAddress { get; set; }

        //Billing&ShippingDetails
        public string? DeliveryTerms { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public int? ShipToId { get; set; }
        //public List<DocumentUploadPostDto>? POFiles { get; set; }
        public string? POFiles { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? Transport { get; set; }
        public string? Others { get; set; }
        public string? SpecialTermsAndConditions { get; set; }
        public bool IsDeleted { get; set; } = false;

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public decimal TotalAmount { get; set; }
        public bool POApprovalI { get; set; }
        public DateTime POApprovedIDate { get; set; }
        public string? POApprovedIBy { get; set; }
        public bool POApprovalII { get; set; }
        public DateTime POApprovedIIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoItemsReportDto>? POItems { get; set; }
        public List<PoIncoTermReportDto>? POIncoTerms { get; set; }

    }
    public class PurchaseOrderSPReportDTO
    {
        public string? VendorName { get; set; }

        public string? PONumber { get; set; }
        public string? PartNumber { get; set; }
        public decimal? Qty { get; set; }
        public decimal? TotalWithTax { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }
        public decimal? BalanceQty { get; set; }


    }

    public class PurchaseOrderSPReportWithParamDTO
    {
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
    }
    public class PurchaseOrderSPReportWithParamForTransDTO
    {
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class PurchaseOrderUnitListSPReportWithParamForTransDTO
    {
        public string? ItemNumber { get; set; }
    }
    public class PurchaseOrderUnitListSPReportWithParamForTrans
    {
        public string? ItemNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Uoc { get; set; }
        public string? VendorName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? PoNumberCount { get; set; }
        public string? RecordType { get; set; }
    }

    public class PurchaseOrderApprovalSPReportWithParamDTO
        {
            public string? VendorName { get; set; }
            public string? PONumber { get; set; }
            public string? ItemNumber { get; set; }
            public string? RecordType { get; set; }
            public string? Postatus { get; set; }
            public string? Approval { get; set; }
        }
    public class PurchaseOrderApprovalSPReportWithDateDTO
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? RecordType { get; set; }
        public string? Approval { get; set; }
    }
    public class PurchaseOrderApprovalSPReportWithParamForTransDTO
    {
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? RecordType { get; set; }
        public string? Postatus { get; set; }
        public string? Approval { get; set; }
        public string? ProjectNumber { get; set; }
    }

    public class Tras_POSPReportDTO
        {
            public string? VendorName { get; set; }
            public string? PONumber { get; set; }
            public string? PartNumber { get; set; }
        }
        public class Data
        {
            public int id { get; set; }
            public string processType { get; set; }
            public string template { get; set; }
            public string subject { get; set; }
        }

        public class EmailTemplateDto
        {
            public Data data { get; set; }
            public string message { get; set; }
            public bool success { get; set; }
            public int statusCode { get; set; }
        }
}
