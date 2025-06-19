namespace Tips.SalesService.Api.Entities
{
    public class QuotationSPReport
    {
        public string? QuoteNumber { get; set; }
        public decimal? QuotationVersionNo { get; set; }
        public string? RfqNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? LeadId { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? MaterialGroup { get; set; }
        public DateTime? QuoteCreatedOn { get; set; }
        public DateTime? QuoteSentOn { get; set; }
        public string? RoomName { get; set; }
        public string? KPN { get; set; }
        public string? KPNDescription { get; set; }
        public string? UOC { get; set; }
        public string? Uom { get; set; }
        public string? PriceList { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? BasicAmount { get; set; }
        public string? DiscountType { get; set; }
        public decimal? TotalFinalAmount { get; set; }
        public decimal? TotalAdditionalCharges { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? InstallationCharges { get; set; }
    }
}
