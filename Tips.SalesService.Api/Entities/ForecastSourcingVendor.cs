using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class ForecastSourcingVendor
    {
        public int Id { get; set; }
        public string? Vendor { get; set; }
        [Precision(13, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitPricePer { get; set; }
        public string? Currency { get; set; }
        public string? MOQ { get; set; }
        public string? LeadTime { get; set; }
        public string? Freight { get; set; }
        public string? Duties { get; set; }
        public int? QuoteQuantity { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteValidity { get; set; }
        public string? UploadFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int ForeCastSourcingItemsId { get; set; }
        public ForecastSourcingItems? ForecastSourcingItems { get; set; }
    }
}
