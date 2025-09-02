using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PoItem
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }

        public string? PONumber { get; set; }

        public decimal BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }

        public PoPartType? PartType { get; set; }
        public string? SpecialInstruction { get; set; }

        public decimal? Allowance { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        public bool PoPartsStatus { get; set; }

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
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public PoStatus PoStatus { get; set; }
        public string? ReasonforShortClose { get; set; }
        public string? Remarks { get; set; }
        public string? drawingRevNo { get; set; }        
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        public List<PoAddProject>? POAddprojects { get; set; }
        public List<PoAddDeliverySchedule>? POAddDeliverySchedules { get; set; }
        public List<PoSpecialInstruction>? POSpecialInstructions { get; set; }
        public List<PoConfirmationDate>? POConfirmationDates { get; set; }
        public List<PrDetails>? PrDetails { get; set; }

    }
}
