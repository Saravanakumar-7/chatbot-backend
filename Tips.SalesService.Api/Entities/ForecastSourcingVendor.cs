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

        [Precision(13,3)]
        public decimal? QuoteQuantity { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteValidity { get; set; }
        public string? UploadFile { get; set; }
        public bool IsActive { get; set; } = true;
        public int ForeCastSourcingItemsId { get; set; }
        public ForecastSourcingItems? ForecastSourcingItems { get; set; }
    }
}
