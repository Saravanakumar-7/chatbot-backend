using static Tips.Production.Api.Entities.ShopOrder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderDto
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }

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
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string? SalesOrderNumber { get; set; }

        [Required]
        public Decimal SalesOrderQty { get; set; }

        [Required]
        public Decimal ShopOrderReleaseQty { get; set; }

        [Required]
        public DateTime ShopOrderCloseDate { get; set; }

        [MaxLength(100)]
        public string? SalesOrderPONumber { get; set; }

        public OrderStatus Status { get; set; }

        public decimal WipQty { get; set; }
        public decimal OqcQty { get; set; }
        public decimal ScrapQty { get; set; }
        public OrderStatus FgDoneStatus { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }


        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public DateTime? ShortClosedOn { get; set; }

        [MaxLength(100)]
        public string? ShorClosedBy { get; set; }
        public IssueStatus MaterialIssueStatus { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }

        [Required]
        public string? Unit { get; set; }

    }

    public class ShopOrderPostDto
    {

        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }

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
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string? SalesOrderNumber { get; set; }

        [Required]
        public Decimal SalesOrderQty { get; set; }

        [Required]
        public Decimal ShopOrderReleaseQty { get; set; }

        [Required]
        public DateTime ShopOrderCloseDate { get; set; }

        [MaxLength(100)]
        public string? SalesOrderPONumber { get; set; }

        public OrderStatus Status { get; set; }

        public decimal WipQty { get; set; }
        public decimal OqcQty { get; set; }
        public decimal ScrapQty { get; set; }
        public OrderStatus FgDoneStatus { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }


        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public DateTime? ShortClosedOn { get; set; }

        [MaxLength(100)]
        public string? ShorClosedBy { get; set; }
        public IssueStatus MaterialIssueStatus { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }

        [Required]
        public string? Unit { get; set; }


    }

    public class ShopOrderUpdateDto
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }

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
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string? SalesOrderNumber { get; set; }

        [Required]
        public Decimal SalesOrderQty { get; set; }

        [Required]
        public Decimal ShopOrderReleaseQty { get; set; }

        [Required]
        public DateTime ShopOrderCloseDate { get; set; }

        [MaxLength(100)]
        public string? SalesOrderPONumber { get; set; }

        public OrderStatus Status { get; set; }

        public decimal WipQty { get; set; }
        public decimal OqcQty { get; set; }
        public decimal ScrapQty { get; set; }
        public OrderStatus FgDoneStatus { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }


        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public DateTime? ShortClosedOn { get; set; }

        [MaxLength(100)]
        public string? ShorClosedBy { get; set; }
        public IssueStatus MaterialIssueStatus { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }

        [Required]
        public string? Unit { get; set; }


    }
}
