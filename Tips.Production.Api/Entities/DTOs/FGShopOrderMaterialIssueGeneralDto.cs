using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class FGShopOrderMaterialIssueGeneralDto
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
    public class FGShopOrderMaterialIssueGeneralPostDto
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
    public class FGShopOrderMaterialIssueGeneralUpdateDto
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
