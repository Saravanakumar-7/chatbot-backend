using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderItemDto
    {
        public int Id { get; set; }

        public string? FGItemNumber { get; set; }

        public string? Description { get; set; }

        public string? ProjectNumber { get; set; }

        public string? SalesOrderNumber { get; set; }

        public decimal? OpenSalesOrderQty { get; set; }
        
        public decimal? ReleaseQty { get; set; }
    }
    public class ShopOrderItemPostDto
    {
        [Required]
        public string? FGItemNumber { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? ProjectNumber { get; set; }
        [Required]
        public string? SalesOrderNumber { get; set; }

        [Precision(13, 3)]
        public decimal? OpenSalesOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? ReleaseQty { get; set; }
    }
    public class ShopOrderItemUpdateDto   
    {
        [Required]
        public string? FGItemNumber { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? ProjectNumber { get; set; }
        [Required]
        public string? SalesOrderNumber { get; set; }

        [Precision(13, 3)]
        public decimal? OpenSalesOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? ReleaseQty { get; set; }
    }
}
