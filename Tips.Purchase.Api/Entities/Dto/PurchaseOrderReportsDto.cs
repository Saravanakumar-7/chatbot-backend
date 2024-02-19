using System.ComponentModel;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class poconfirmation_report_Dto
    {
        public int? pcdID { get; set; }
        public DateTime? pcdConfirmationDate { get; set; }
        public decimal? pcdQty { get; set; }
        public int? pcdPOItemDetailId { get; set; }
        public int? poiId { get; set; }
        public string? poiItemNumber { get; set; }
        public string? poiMftrItemNumber { get; set; }
        public string? poiDescription { get; set; }
        public string? poiUOM { get; set; }
        public decimal? poiUnitPrice { get; set; }
        public decimal? poiQty { get; set; }
        public string? poiPONumber { get; set; }
        public decimal? poiBalanceQty { get; set; }
        public int? poiPartType { get; set; }
        public string? poiSpecialInstruction { get; set; }
        public int? poiIsTechnicalDocsRequired { get; set; }
        public int? poiPoPartsStatus { get; set; }
        public decimal? poiSGST { get; set; }
        public decimal? poiCGST { get; set; }
        public decimal? poiIGST { get; set; }
        public decimal? poiUTGST { get; set; }
        public decimal? poiSubTotal { get; set; }
        public decimal? poiTotalWithTax { get; set; }
        public string? poiCreatedBy { get; set; }
        public DateTime? poiCreatedOn { get; set; }
        public string? poiLastModifiedBy { get; set; }
        public DateTime? poiLastModifiedOn { get; set; }
        public int? poiPurchaseOrderId { get; set; }
        public decimal? poiReceivedQty { get; set; }
        public int? poiPoStatus { get; set; }
        public int Id { get; set; }
        public string? PONumber { get; set; }

        public string? AmountInWords { get; set; }

        public DateTime? PODate { get; set; }
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
        public string? BillToId { get; set; }
        public string? ShipToId { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }

        public decimal? TotalAmount { get; set; }

        public bool? POApprovalI { get; set; } = false;
        public string? POApprovedIBy { get; set; }
        public DateTime? POApprovedIDate { get; set; }
        public bool? POApprovalII { get; set; } = false;
        public string? POApprovedIIBy { get; set; }
        public DateTime? POApprovedIIDate { get; set; }
        public bool? IsDeleted { get; set; } = false;

        [DefaultValue(0)]
        public Status? Status { get; set; }

        [DefaultValue(false)]
        public bool? IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public PoStatus? PoStatus { get; set; }
        [DefaultValue(false)]
        public bool? PoConfirmationStatus { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public bool? IsModified { get; set; } = false;
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        //public string? POFiles { get; set; }
    }
    public class podeliveryschedule_report_Dto
    {
        public int? podId { get; set; }
        public DateTime? podPODeliveryDate { get; set; }
        public decimal? podPODeliveryQty { get; set; }
        public int? podPOItemDetailId { get; set; }
        public int? poiId { get; set; }
        public string? poiItemNumber { get; set; }
        public string? poiMftrItemNumber { get; set; }
        public string? poiDescription { get; set; }
        public string? poiUOM { get; set; }
        public decimal? poiUnitPrice { get; set; }
        public decimal? poiQty { get; set; }
        public string? poiPONumber { get; set; }
        public decimal? poiBalanceQty { get; set; }
        public int? poiPartType { get; set; }
        public string? poiSpecialInstruction { get; set; }
        public int? poiIsTechnicalDocsRequired { get; set; }
        public int? poiPoPartsStatus { get; set; }
        public decimal? poiSGST { get; set; }
        public decimal? poiCGST { get; set; }
        public decimal? poiIGST { get; set; }
        public decimal? poiUTGST { get; set; }
        public decimal? poiSubTotal { get; set; }
        public decimal? poiTotalWithTax { get; set; }
        public string? poiCreatedBy { get; set; }
        public decimal? poiCreatedOn { get; set; }
        public string? poiLastModifiedBy { get; set; }
        public DateTime? poiLastModifiedOn { get; set; }
        public int? poiPurchaseOrderId { get; set; }
        public decimal? poiReceivedQty { get; set; }
        public int? poiPoStatus { get; set; }
        public int Id { get; set; }
        public string? PONumber { get; set; }

        public string? AmountInWords { get; set; }

        public DateTime? PODate { get; set; }
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
        public string? BillToId { get; set; }
        public string? ShipToId { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }

        public decimal? TotalAmount { get; set; }

        public bool? POApprovalI { get; set; } = false;
        public string? POApprovedIBy { get; set; }
        public DateTime? POApprovedIDate { get; set; }
        public bool? POApprovalII { get; set; } = false;
        public string? POApprovedIIBy { get; set; }
        public DateTime? POApprovedIIDate { get; set; }
        public bool? IsDeleted { get; set; } = false;

        [DefaultValue(0)]
        public Status? Status { get; set; }

        [DefaultValue(false)]
        public bool? IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public PoStatus? PoStatus { get; set; }
        [DefaultValue(false)]
        public bool? PoConfirmationStatus { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public bool? IsModified { get; set; } = false;
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        //public string? POFiles { get; set; }
    }
    public class poproject_report_Dto
    {
        public int? pjId { get; set; }
        public string? pjProjectNumber { get; set; }
        public decimal? pjProjectQty { get; set; }
        public int? pjPOItemDetailId { get; set; }
        public int? poiId { get; set; }
        public string? poiItemNumber { get; set; }
        public string? poiMftrItemNumber { get; set; }
        public string? poiDescription { get; set; }
        public string? poiUOM { get; set; }
        public decimal? poiUnitPrice { get; set; }
        public decimal? poiQty { get; set; }
        public string? poiPONumber { get; set; }
        public decimal? poiBalanceQty { get; set; }
        public int? poiPartType { get; set; }
        public string? poiSpecialInstruction { get; set; }
        public int? poiIsTechnicalDocsRequired { get; set; }
        public int? poiPoPartsStatus { get; set; }
        public decimal? poiSGST { get; set; }
        public decimal? poiCGST { get; set; }
        public decimal? poiIGST { get; set; }
        public decimal? poiUTGST { get; set; }
        public decimal? poiSubTotal { get; set; }
        public decimal? poiTotalWithTax { get; set; }
        public string? poiCreatedBy { get; set; }
        public DateTime? poiCreatedOn { get; set; }
        public string? poiLastModifiedBy { get; set; }
        public DateTime? poiLastModifiedOn { get; set; }
        public int? poiPurchaseOrderId { get; set; }
        public decimal? poiReceivedQty { get; set; }
        public int? poiPoStatus { get; set; }
        public int Id { get; set; }
        public string? PONumber { get; set; }

        public string? AmountInWords { get; set; }

        public DateTime? PODate { get; set; }
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
        public string? BillToId { get; set; }
        public string? ShipToId { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }

        public decimal? TotalAmount { get; set; }

        public bool? POApprovalI { get; set; } = false;
        public string? POApprovedIBy { get; set; }
        public DateTime? POApprovedIDate { get; set; }
        public bool? POApprovalII { get; set; } = false;
        public string? POApprovedIIBy { get; set; }
        public DateTime? POApprovedIIDate { get; set; }
        public bool? IsDeleted { get; set; } = false;

        [DefaultValue(0)]
        public Status? Status { get; set; }

        [DefaultValue(false)]
        public bool? IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public PoStatus? PoStatus { get; set; }
        [DefaultValue(false)]
        public bool? PoConfirmationStatus { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public bool? IsModified { get; set; } = false;
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class PurchaseOrder_ReportDto
    {
        public List<poconfirmation_report_Dto>? poconfirmation_Report_Dtos { get; set; }
        public List<podeliveryschedule_report_Dto>? podeliveryschedule_Report_Dtos { get; set; }
        public List<poproject_report_Dto>? poproject_Report_Dtos { get; set; }
    }

}
