using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel; 
using Tips.Production.Api.Entities.Enums;
using Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class SAShopOrder
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? SAShopOrderNumber { get; set; }
        [Required]
        [MaxLength(50)]
        public string? ProjectType { get; set; }
        [Required]
        [MaxLength(50)]
        public string? ProjectNumber { get; set; }
        [Required]
        [MaxLength(100)]
        public string? FGItemNumber { get; set; }
        [Required]
        [MaxLength(100)]
        public string? SAItemNumber { get; set; }

        [Required]
        public PartType ItemType { get; set; }

        [Precision(13, 3)]
        public decimal TotalSOReleaseQty { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(50)]
        public string? SalesOrderNumber { get; set; }
        [Precision(13, 3)]
        [Required]
        public decimal SalesOrderQty { get; set; }

        [Required]
        public decimal BomRevisionNo { get; set; }

        [Precision(13, 3)]
        [Required]
        public decimal SAShopOrderReleaseQty { get; set; }
        [Required]
        public DateTime? SAShopOrderCloseDate { get; set; }
        [MaxLength(100)]
        public string? SalesOrderPONumber { get; set; }
        [DefaultValue(0)]
        public OrderStatus Status { get; set; }
        [Precision(13, 3)]
        public decimal WipQty { get; set; }
        [Precision(13, 3)]
        public decimal OqcQty { get; set; }
        [Precision(13, 3)]
        public decimal ScrapQty { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        [MaxLength(100)]
        public string? ShorClosedBy { get; set; }
        [DefaultValue(0)]
        public IssuedStatus MaterialIssueStatus { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SAShopOrderItem>? ShopOrderItems { get; set; }

    }
}
