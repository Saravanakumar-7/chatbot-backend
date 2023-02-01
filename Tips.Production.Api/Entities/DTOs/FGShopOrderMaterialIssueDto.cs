using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class FGShopOrderMaterialIssueDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGPartNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public string? ShopOrderType { get; set; }
        [MinLength(500)]
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<FGShopOrderMaterialIssueGeneralDto>? FGShopOrderMaterialIssueGeneralDtos { get; set; }
    }
    public class FGShopOrderMaterialIssuePostDto
    {
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGPartNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public string? ShopOrderType { get; set; }
        [MinLength(500)]
        public string? Description { get; set; }
        public List<FGShopOrderMaterialIssueGeneralPostDto>? FGShopOrderMaterialIssueGeneralPostDtos { get; set; }
    }
    public class FGShopOrderMaterialIssueUpdateDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGPartNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        public string? ShopOrderType { get; set; }
        [MinLength(500)]
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<FGShopOrderMaterialIssueGeneralUpdateDto>? FGShopOrderMaterialIssueGeneralUpdateDtos { get; set; }
    }
}
