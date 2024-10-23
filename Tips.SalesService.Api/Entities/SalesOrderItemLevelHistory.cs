using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderItemLevelHistory
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public int SalesOrderItemId { get; set; }
        public int? RevisionNumber { get; set; }
        public string? CustomerItemNumber { get; set; }
        public string? Description { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType PartType { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public decimal? ShortClosedQty { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public OrderStatus StatusEnum { get; set; }
        public string? UOM { get; set; }
        public string? Currency { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }
        [Precision(13, 3)]
        public decimal? Discount { get; set; }
        public string? RoomName { get; set; }
        public string? DiscountType { get; set; }

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
        public string? PriceList { get; set; }
        public string? Remarks { get; set; }
        public int SalesOrderMainLevelHistoryId { get; set; }
        public SalesOrderMainLevelHistory? SalesOrderMainLevelHistory { get; set; }
        public List<SalesOrderScheduleDateHistory>? SalesOrderScheduleDateHistory { get; set; }
    }
}
