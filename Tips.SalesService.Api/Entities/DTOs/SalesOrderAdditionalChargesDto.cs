using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Entities.Enum;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SalesOrderAdditionalChargesDto
    {
        [Key]
        public int Id { get; set; }
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 3)]
        public decimal? AddtionalChargesValueAmount { get; set; }

        [Precision(13, 3)]
        public decimal? TotalValue { get; set; }

        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }
        public SoStatus SOAdditionalStatus { get; set; }

    }

    public class SalesOrderAdditionalChargesPostDto
    {
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 3)]
        public decimal? AddtionalChargesValueAmount { get; set; }

        [Precision(13, 3)]
        public decimal? TotalValue { get; set; }

        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }
        public SoStatus SOAdditionalStatus { get; set; }

    }
    public class SalesOrderAdditionalChargesUpdateDto
    {
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 3)]
        public decimal? AddtionalChargesValueAmount { get; set; }

        [Precision(13, 3)]
        public decimal? TotalValue { get; set; }

        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }
        public SoStatus SOAdditionalStatus { get; set; }

    }
    public class SalesOrderAdditionalChargesReportDto
    {
        public int Id { get; set; }
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 3)]
        public decimal? AddtionalChargesValueAmount { get; set; }

        [Precision(13, 3)]
        public decimal? TotalValue { get; set; }

        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }

    }
}
