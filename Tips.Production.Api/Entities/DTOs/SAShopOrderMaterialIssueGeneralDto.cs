using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class SAShopOrderMaterialIssueGeneralDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        [MinLength(500)]
        public string? Description { get; set; }
        public string? PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 3)]
        public decimal AlreadyIssuedQty { get; set; }
        [Precision(13, 3)]
        public decimal IssueQty { get; set; }
    }
    public class SAShopOrderMaterialIssueGeneralPostDto
    {
        public string? PartNumber { get; set; }
        [MinLength(500)]
        public string? Description { get; set; }
        public string? PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 3)]
        public decimal AlreadyIssuedQty { get; set; }
        [Precision(13, 3)]
        public decimal IssueQty { get; set; }
    }
    public class SAShopOrderMaterialIssueGeneralUpdateDto
    {
        public string? PartNumber { get; set; }
        [MinLength(500)]
        public string? Description { get; set; }
        public string? PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 3)]
        public decimal AlreadyIssuedQty { get; set; }
        [Precision(13, 3)]
        public decimal IssueQty { get; set; }
    }
}
