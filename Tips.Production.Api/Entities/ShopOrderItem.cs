using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities
{
    public class ShopOrderItem
    {
        [Key]
        public int Id { get; set; }

        public string? FGItemNumber { get; set; }

        public string? Description { get; set; }

        public string? ProjectNumber { get; set; }

        public string? SalesOrderNumber { get; set; }

        [Precision(13,3)]
        public decimal? OpenSalesOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? ReleaseQty { get; set; }

        [Precision(13, 3)]
        public decimal? RequiredQty { get; set; }
        public int ShopOrderId { get; set; }
        public ShopOrder ShopOrder { get; set; }

    }
}
