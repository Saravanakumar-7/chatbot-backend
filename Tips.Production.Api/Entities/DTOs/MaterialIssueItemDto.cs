using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialIssueItemDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }

        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        [Precision(13, 3)]
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; } 
        public int MaterialIssueId { get; set; }
        
    }

    public class MaterialIssueItemPostDto
    {
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
          
    }

    public class MaterialIssueItemUpdateDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; } 
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }

        [Precision(13, 3)]
        public decimal NewIssueQty { get; set; }

        [Precision(13, 3)]
        public string? Unit { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public int MaterialIssueId { get; set; }
        
    }

}
