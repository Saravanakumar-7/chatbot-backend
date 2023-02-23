using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tips.Production.Api.Entities.DTOs
{
    public class SAShopOrderDto
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
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(50)]
        public string? SalesOrderNumber { get; set; }
        [Precision(13, 3)]
        [Required]
        public decimal SalesOrderQty { get; set; }
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
        public IssueStatus MaterialIssueStatus { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class SAShopOrderPostDto
    {
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
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(50)]
        public string? SalesOrderNumber { get; set; }
        [Precision(13, 3)]
        [Required]
        public decimal SalesOrderQty { get; set; }
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
        public IssueStatus MaterialIssueStatus { get; set; }
      
    }

    public class SAShopOrderUpdateDto
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
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(50)]
        public string? SalesOrderNumber { get; set; }
        [Precision(13, 3)]
        [Required]
        public decimal SalesOrderQty { get; set; }
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
        public IssueStatus MaterialIssueStatus { get; set; }
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}
