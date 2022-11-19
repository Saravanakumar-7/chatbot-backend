using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tips.Production.Api.Entities
{
    public class ShopOrder
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ShopOrderNo { get; set; }
        

        [Required]
        [MaxLength(50)]
        public string? ProjectType { get; set; }

        [Required]
        [MaxLength(50)]
        public string? ProjectNo { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FGItemNo { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string? SalesOrderNo { get; set; }
       

        [Precision(13,2)]
        [Required]
        public decimal SalesOrderQty { get; set; }

        [Precision(13, 2)]
        [Required]
        public decimal ShopOrderReleaseQty { get; set; }

        [Required]
        public DateTime? ShopOrderCloseDate { get; set; }

        [MaxLength(100)]
        public string? SalesOrderPoNo { get; set; }

        [DefaultValue(0)]
        public OrderStatus Status { get; set; }

        [Precision(13, 2)]
        public decimal WipQty { get; set; }
        [Precision(13, 2)]
        public decimal OqcQty { get; set; }
        [Precision(13, 2)]
        public decimal ScrapQty { get; set; }
        [DefaultValue(0)]
        public OrderStatus FgDoneStatus { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }


        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public DateTime? ShortClosedOn { get; set; }

        [MaxLength(100)]
        public string? ShorClosedBy { get; set; }

        [DefaultValue(0)]
        public IssueStatus MaterialIssueStatus { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public string? Unit { get; set; }

        
    }
}
