using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class SAShopOrderItem
    {
        [Key]
        public int Id { get; set; }

        public string? ItemNumber { get; set; }

        public string? Description { get; set; }

        public string? ProjectNumber { get; set; }

        public string? SASalesOrderNumber { get; set; }

        [Precision(13, 3)]
        public decimal? OpenSASalesOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? ReleaseQty { get; set; }

        [Precision(13, 3)]
        public decimal? RequiredQty { get; set; }
        public OrderStatus Status { get; set; }
        public int SAShopOrderId { get; set; }
        public SAShopOrder SAShopOrder { get; set; }

    }
}
