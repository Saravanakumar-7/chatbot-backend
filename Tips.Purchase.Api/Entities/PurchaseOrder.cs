using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string? RevisionNumber { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public string? POFiles { get; set; }

        //VendorDetails
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? QuotationReferenceNumber { get; set; }
        public DateTime QuotationDate { get; set; }
        public string? VendorAddress { get; set; }

        //Billing&ShippingDetails
        public string? DeliveryTerms { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }

        //Terms
        public string? RetentionPeriod { get; set; }
        public string? SpecialTermsAndConditions { get; set; }

        public decimal TotalAmount { get; set; }

        public bool POApprovalI { get; set; } = false;
        public string? POApprovedIBy { get; set; }
        public DateTime POApprovedIDate { get; set; }
        public bool POApprovalII { get; set; }=false;
        public string? POApprovedIIBy { get; set; }
        public DateTime POApprovedIIDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        [DefaultValue(0)]
        public Status Status { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoItem>? POItemList { get; set; }

    }
}
