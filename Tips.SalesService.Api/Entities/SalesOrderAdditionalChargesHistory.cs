using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderAdditionalChargesHistory
    {
        [Key]
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

        [Precision(13, 3)]
        public decimal? TotalValue { get; set; }

        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }
        public string? SalesOrderNumber { get; set; }
        public int? RevisionNumber { get; set; }
    }
}
