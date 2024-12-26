using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteAdditionalChargesDto
    {
        public int Id { get; set; }
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 3)]
        public decimal? AddtionalChargesValueAmount { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }
        public string? Remarks { get; set; }
    }
    public class QuoteAdditionalChargesPostDto
    {
       
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 3)]
        public decimal? AddtionalChargesValueAmount { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }
        public string? Remarks { get; set; }
    }
    public class QuoteAdditionalChargesUpdateDto
    { 
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 3)]
        public decimal? AddtionalChargesValueAmount { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }
        public string? Remarks { get; set; }
    }
}
