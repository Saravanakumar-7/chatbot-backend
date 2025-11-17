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

        // Fields from rfqsourcingitems
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? QtyReq { get; set; }
        public decimal? Count { get; set; }
        public string? Manufacturer_Mftr_PartNumber { get; set; }
        public string? Customer_Mftr_PartNumber { get; set; }
    }

    public class SourcingInputParam
    {
        public string? RFQNumber { get; set; }
        public string? ItemNumber { get; set; }
    }
}
