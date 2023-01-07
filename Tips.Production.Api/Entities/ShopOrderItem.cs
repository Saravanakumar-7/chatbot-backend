using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities
{
    public class ShopOrderItem
    {
        [Key]
        public int Id { get; set; }

        public string? ItemNumber { get; set; }

        public string? Description { get; set; }

        public string? ProjectNumber { get; set; }

        public string? SalesOrderNumber { get; set; }

        [Precision(13,3)]
        public decimal? OpenSalesOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? ReleaseQty { get; set; }

    }
}
