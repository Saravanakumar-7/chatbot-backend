using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SourcingVendorDto
    {
        public int Id { get; set; }

        public string? SourcingVendors { get; set; }

        public string? SourcingCurrency { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }

        public DateTime? QuoteDate { get; set; }

        public string? UnitPrice { get; set; }

        public string? MOQ { get; set; }
        [Precision(13, 3)]
        public decimal? Duties { get; set; }

        public DateTime? QuoteValidity { get; set; }
        public string? UnitPricePer { get; set; }
        public string? LeadTime { get; set; }
        public string? QuoteQuantity { get; set; }
        public string? UploadFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class SourcingVendorPostDto
    { 
        public string? SourcingVendors { get; set; }

        public string? SourcingCurrency { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }

        public DateTime? QuoteDate { get; set; }

        public string? UnitPrice { get; set; }

        public string? MOQ { get; set; }
        [Precision(13, 3)]
        public decimal? Duties { get; set; }

        public DateTime? QuoteValidity { get; set; }
        public string? UnitPricePer { get; set; }
        public string? LeadTime { get; set; }
        public string? QuoteQuantity { get; set; }
        public string? UploadFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class SourcingVendorUpdateDto
    {
        public int Id { get; set; }

        public string? SourcingVendors { get; set; }

        public string? SourcingCurrency { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }

        public DateTime? QuoteDate { get; set; }

        public string? UnitPrice { get; set; }

        public string? MOQ { get; set; }
        [Precision(13, 3)]
        public decimal? Duties { get; set; }

        public DateTime? QuoteValidity { get; set; }
        public string? UnitPricePer { get; set; }
        public string? LeadTime { get; set; }
        public string? QuoteQuantity { get; set; }
        public string? UploadFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }


}
