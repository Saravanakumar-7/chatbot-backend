using static Tips.Production.Api.Entities.ShopOrder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Entities.Enums;
using Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }

        public ProjectType ProjectType { get; set; }
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? TotalSOReleaseQty { get; set; }
        public DateTime SOCloseDate { get; set; }
        public decimal? CanCreateQty { get; set; }

        public decimal? WipQty { get; set; }

        public decimal? OqcQty { get; set; }

        public decimal? ScrapQty { get; set; }

        public OrderStatus FGDoneStatus { get; set; }
        public bool IsDeleted { get; set; } = false;

        public OrderStatus Status { get; set; }

        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ShopOrderItemDto>? ShopOrderItems { get; set; }

    }

    public class ShopOrderPostDto
    {
        [Required]
        public ProjectType ProjectType { get; set; }
        [Required]
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal? TotalSOReleaseQty { get; set; }
        [Required]
        public DateTime SOCloseDate { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }
        [Precision(13, 3)]
        public decimal? CanCreateQty { get; set; }

        public List<ShopOrderItemPostDto>? ShopOrderItems { get; set; }
    }

    public class ShopOrderUpdateDto
    {
        public int Id { get; set; }
        [Required]
        public string? ShopOrderNumber { get; set; }
        [Required]
        public ProjectType ProjectType { get; set; }
        [Required]
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal? TotalSOReleaseQty { get; set; }
        [Required]
        public DateTime SOCloseDate { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }
        [Precision(13, 3)]
        public decimal? CanCreateQty { get; set; }
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ShopOrderItemUpdateDto>? ShopOrderItems { get; set; }
    }
    public class ListOfShopOrderDto
    {
       public int Id { get; set; }
       public string? ShopOrderNumber { get; set; }
    }

}
