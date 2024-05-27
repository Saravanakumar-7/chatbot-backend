using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PoItemHistory
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
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public int PoItemId { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }

        public decimal? BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }
        public decimal? ShortClosedQty { get; set; }

        public PoPartType? PartType { get; set; }
        public string? SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        public bool PoPartsStatus { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }
        [Precision(13, 3)]
        public decimal? CGST { get; set; }
        [Precision(13, 3)]
        public decimal? IGST { get; set; }
        [Precision(13, 3)]
        public decimal? UTGST { get; set; }


        [Precision(13, 3)]
        public decimal? SubTotal { get; set; }

        [Precision(13, 3)]
        public decimal? TotalWithTax { get; set; }
        public PoStatus PoItemStatus { get; set; }
        public string? ReasonforShortClose { get; set; }
        public string? Remarks { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}
