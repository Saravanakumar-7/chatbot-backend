namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SourcingSpReport
    {
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? QtyReq { get; set; }
        public decimal? Count { get; set; }
        public string? Manufacturer_Mftr_PartNumber { get; set; }
        public string? Customer_Mftr_PartNumber { get; set; }

        public string? Vendor { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UnitPricePer { get; set; }
        public string? Currency { get; set; }
        public string? MOQ { get; set; }
        public string? LeadTime { get; set; }
        public decimal? Freight { get; set; }
        public decimal? Duties { get; set; }
        public decimal? QuoteQty { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteValidity { get; set; }
        public bool? Primary { get; set; }
        public decimal? LandingPrice { get; set; }
        public decimal? MoqCost { get; set; }
        public string? VendorId { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Remarks { get; set; }
        public string? FileIds { get; set; }
    }

    public class SourcingInputParam
    {
        public string? RFQNumber { get; set; }
        public string? ItemNumber { get; set; }
    }
}
