namespace Tips.Purchase.Api.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string RevNumber { get; set; }
        public string ProcurementType { get; set; }
        public string Currency { get; set; }
        public string POFiles { get; set; }

        //VendorDetails
        public string VendorName { get; set; }
        public string VendorId { get; set; }
        public string QuotationReferenceNo { get; set; }
        public DateTime QuotationDated { get; set; }
        public string VendorAddress { get; set; }

        //Billing&ShippingDetails
        public string DeliveryTerms { get; set; }
        public string PaymentTerms { get; set; }
        public string ShippingMode { get; set; }
        public string ShipTo { get; set; }
        public string BillTo { get; set; }

        //Terms
        public string RetentionPeriod { get; set; }
        public string SpecialTermsAndConditions { get; set; }

        public bool POApprovalI { get; set; } = false;
        public string? POApprovedIBy { get; set; }
        public DateTime POApprovedIDate { get; set; }

        public bool POApprovalII { get; set; }=false;
        public string? POApprovedIIBy { get; set; }
        public DateTime POApprovedIIDate { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoItems>? poItems { get; set; }

    }
}
