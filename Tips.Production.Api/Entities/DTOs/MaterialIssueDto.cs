using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialIssueDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string? ProjectNumber { get; set; }

        public string ItemType { get; set; }

        [Precision(13, 2)]
        public decimal ShopOrderQty { get; set; }
        public string? ShopOrderType { get; set; }

        public string PartNumber { get; set; }
        [MinLength(500)]
        public string Description { get; set; }
        public string PartType { get; set; }
        public string UOM { get; set; }
        [Precision(13, 2)]
        public decimal Qty { get; set; }
        [Precision(13, 2)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 2)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 2)]
        public decimal AlreadyIssuedQty { get; set; }
        [Precision(13, 2)]
        public decimal IssueQty { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class MaterialIssueDtoPost
    {
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrderDate { get; set; }

        [Required]
        public string? ProjectNumber { get; set; }

        [Required]
        public string ItemType { get; set; }

        [Precision(13, 2)]
        public decimal ShopOrderQty { get; set; }
        public string? ShopOrderType { get; set; }

        [Required]
        public string PartNumber { get; set; }
        [MinLength(500)]
        public string Description { get; set; }
        public string UOM { get; set; }

        [Precision(13, 2)]
        public decimal Qty { get; set; }
        [Precision(13, 2)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 2)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 2)]
        public decimal AlreadyIssuedQty { get; set; }
        [Precision(13, 2)]
        public decimal IssueQty { get; set; }       

    }

    public class MaterialIssueDtoUpdate
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrderDate { get; set; }

        [Required]
        public string? ProjectNumber { get; set; }

        [Required]
        public string ItemType { get; set; }

        [Precision(13, 2)]
        public decimal ShopOrderQty { get; set; }
        public string? ShopOrderType { get; set; }

        [Required]
        public string PartNumber { get; set; }
        [MinLength(500)]
        public string Description { get; set; }
        public string UOM { get; set; }

        [Precision(13, 2)]
        public decimal Qty { get; set; }
        [Precision(13, 2)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 2)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 2)]
        public decimal AlreadyIssuedQty { get; set; }
        [Precision(13, 2)]
        public decimal IssueQty { get; set; }

    }
}
