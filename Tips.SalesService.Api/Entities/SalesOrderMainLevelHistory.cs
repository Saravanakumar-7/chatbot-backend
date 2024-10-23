using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tips.SalesService.Api.Entities.Enum;

namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderMainLevelHistory
    {
        [Key]
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public int SalesOrderId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }

        [Precision(13, 1)]
        public int? QuoteRevisionNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

        [Precision(13, 1)]
        public int? RevisionNumber { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }

        public OrderStatus SOStatus { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }

        //PO Details
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        //Billing&Shipping

        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int? ShipToId { get; set; }
        public string? PaymentTerms { get; set; }

        [Precision(18, 3)]
        public decimal? Total { get; set; }

        public string? Remarks { get; set; }
        public string Unit { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }
        public string? ReasonForModification { get; set; }
        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }

        [Precision(13, 3)]
        public decimal? TotalAmountWithInstallationCharges { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }
        [DefaultValue(false)]
        public bool ConfirmStatus { get; set; }
        [DefaultValue(false)]
        public bool ApproveStatus { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool SoConfirmationStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SalesOrderItemLevelHistory>? SalesOrderItemLevelHistory { get; set; }
        public List<SOAdditionalChargesHistory>? SOAdditionalChargesHistory { get; set; }
    }
}
