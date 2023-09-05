using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PoConfirmationHistory
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public string? PONumber { get; set; }

        public decimal BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }

        public PartType? PartType { get; set; }
        public string? SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        public bool PoPartsStatus { get; set; }
        public PoStatus PoStatus { get; set; }

        [Precision(13, 3)]
        public decimal SGST { get; set; }
        [Precision(13, 3)]
        public decimal CGST { get; set; }
        [Precision(13, 3)]
        public decimal IGST { get; set; }
        [Precision(13, 3)]
        public decimal UTGST { get; set; }
        [Precision(13, 3)]
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal TotalWithTax { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
