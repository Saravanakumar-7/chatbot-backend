using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public string? PONumber { get; set; }
        
        public string? AmountInWords { get; set; }

        public DateTime PODate { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public string? CompanyAliasName { get; set; }
        public string? PoTransports { get; set; }
        public string? Other { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
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

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }

        public decimal TotalAmount { get; set; }

        public bool POApprovalI { get; set; } = false;
        public string? POApprovedIBy { get; set; }
        public DateTime POApprovedIDate { get; set; }
        public bool POApprovalII { get; set; } = false;
        public string? POApprovedIIBy { get; set; }
        public DateTime POApprovedIIDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        [DefaultValue(0)]
        public Status Status { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public PoStatus PoStatus { get; set; }
        [DefaultValue(false)]
        public bool PoConfirmationStatus { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public bool IsModified { get; set; } = false;
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<DocumentUpload>? POFiles { get; set; }

        public List<PoItem>? POItems { get; set; }
        public List<PoIncoTerm>? POIncoTerms { get; set; }

    }
}
