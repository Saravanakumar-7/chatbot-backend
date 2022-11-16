namespace Tips.SalesService.Api.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; }
        public string PODate { get; set; }
        public string RevNumber { get; set; }
        public string ProcurementType { get; set; }
        public string Currency { get; set; }
        public string POFiles { get; set; }

        //VendorDetails
        public string VendorName { get; set; }
        public string VendorId { get; set; }
        public string QuotationReferenceNo { get; set; }
        public string QuotationDated { get; set; }
        public string VendorAddress { get; set; }

        //Items
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public string UnitPrice { get; set; }
        public string Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool Wanttoattachedthetechnicaldoc { get; set; }

        //TaxDetails
        public int SGST { get; set; }
        public int CGST { get; set; }
        public int IGST { get; set; }
        public int UTGST { get; set; }
        public int Total { get; set; }

        //Billing&ShippingDetails
        public string DeliveryTerms { get; set; }
        public string PaymentTerms { get; set; }
        public string ShippingModel { get; set; }
        public string ShipTo { get; set; }
        public string BillTo { get; set; }

        //Terms
        public string RetentionPeriod { get; set; }
        public string SpecialTermsAndConditions { get; set; }

    }
}
