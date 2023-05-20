using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderHistory
    {
        [Key]
        public int Id { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }

        public bool? SOStatus { get; set; } = false;


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

        public string Unit { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }    

        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public OrderStatus StatusEnum { get; set; }

        [Precision(13, 3)]
        public decimal? BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal? DispatchQty { get; set; }

        [Precision(13, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(18, 3)]
        public decimal? Discount { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal? OrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }
        [Precision(13, 3)]
        public decimal? CGST { get; set; }
        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        [Precision(13, 3)]
        public decimal? IGST { get; set; }
         

        public DateTime RequestedDate { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
