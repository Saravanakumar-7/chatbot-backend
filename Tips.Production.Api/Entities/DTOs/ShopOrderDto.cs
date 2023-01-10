using static Tips.Production.Api.Entities.ShopOrder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderDto
    {
        public int Id { get; set; }
        public string ShopOrderNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string ProjectType { get; set; }
        public string ItemType { get; set; }

        [Precision(13, 3)]
        public decimal? TotalSOReleaseQty { get; set; }
        public DateTime SOClosedDate { get; set; }
        public string? SAItemNumber { get; set; }       

        [Precision(13, 3)]
        public decimal? CanCreateQty { get; set; }

        [Precision(13, 3)]
        public decimal? WipQty { get; set; }

        [Precision(13, 3)]
        public decimal? OqcQty { get; set; }

        [Precision(13, 3)]
        public decimal? Scrapqty { get; set; }

        [Precision(13, 3)]
        public decimal? SOReleaseQty { get; set; }
        [DefaultValue(0)]
        public OrderStatus FgDoneStatus { get; set; }

        public bool IsDeleted { get; set; } = false;

        [DefaultValue(0)]
        public OrderStatus Status { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public List<ShopOrderItemDto>? ShopOrderItems { get; set; }

    }

    public class ShopOrderDtoPost
    {
        [Required]
        public string ShopOrderNumber { get; set; }
        public string? SalesOrderNumber { get; set; }

        [Required]
        public string ProjectType { get; set; }

        [Required]
        public string ItemType { get; set; }

       

        [Precision(13, 3)]
        public decimal? TotalSOReleaseQty { get; set; }

        [Required]
        public DateTime SOClosedDate { get; set; }
        public string? SAItemNumber { get; set; }  

        [Precision(13, 3)]
        public decimal? CanCreateQty { get; set; }

        public List<ShopOrderItemDtoPost>? ShopOrderItems { get; set; }
    }

    public class ShopOrderDtoUpdate
    {

        public int Id { get; set; }
        [Required]
        public string ShopOrderNumber { get; set; }
        public string? SalesOrderNumber { get; set; }

        [Required]
        public string ProjectType { get; set; }

        [Required]
        public string ItemType { get; set; }

        [Precision(13, 3)]
        public decimal? TotalSOReleaseQty { get; set; }

        [Required]
        public DateTime SOClosedDate { get; set; }
        public string? SAItemNumber { get; set; }       

        [Precision(13, 3)]
        public decimal? CanCreateQty { get; set; }
        public List<ShopOrderItemDtoUpdate>? ShopOrderItems { get; set; }


    }
}
