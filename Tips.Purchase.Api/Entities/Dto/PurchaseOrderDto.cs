using System.ComponentModel;
using Tips.Purchase.Api.Entities.Dto;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; } 

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
        //public List<DocumentUploadPostDto>? POFiles { get; set; }
        public List<DocumentUploadDto>? POFiles { get; set; }

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
        public bool POApprovalII { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoItemsDto>? POItems { get; set; }
        
    }

    public class PurchaseOrderPostDto
    {
        public DateTime PODate { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        public List<DocumentUploadPostDto> POFiles { get; set; }

        //public string? POFiles { get; set; }

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


        public List<PoItemsPostDto>? POItems { get; set; }
       
    }
    public class PurchaseOrderUpdateDto
    {
        
        public string? PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string? ProcurementType { get; set; }
        public string? Currency { get; set; }
        //public List<DocumentUploadUpdateDto> POFiles { get; set; }

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

        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoItemsUpdateDto>? POItems { get; set; }
       
    }
    public class PurchaseOrderIdNameListDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
    }

    public class PurchaseOrderSearchDto
    {
        public List<string>? PONumber { get; set; }
        public List<string>? ProcurementType { get; set; }
        public List<string>? ShippingMode { get; set; }
        public List<string>? VendorName { get; set; }
        public List<Status>? Status { get; set; }

    }

}
