using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PurchaseOrderAdditionalCharges
    {
        [Key]
        public int Id { get; set; }
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }

        [Precision(18, 4)]
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

        public string? Remarks { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}
