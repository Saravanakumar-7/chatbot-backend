using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderItems
    {
        [Key]
        public int Id { get; set; } 
        public string? ItemNumber { get; set; }

        public string? Description { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public OrderStatus StatusEnum { get; set; }       
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        public string? Discount { get; set; }

        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal OrderQty { get; set; }
        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }

        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }

        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }

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
        public int SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; } 
        public List<ScheduleDate>? ScheduleDates { get; set; }

    }
}
