using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PoAddKitProjectDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? Description { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? DrawingRevNo { get; set; }
        public PoPartType PartType { get; set; }
        public string? UOM { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal KitComponentQty { get; set; }
        public decimal KitComponentUnitPrice { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public PoStatus PoAddKitProjectStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class PoAddKitProjectPostDto
    {
        public string? PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? Description { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? DrawingRevNo { get; set; }
        public PoPartType PartType { get; set; }
        public string? UOM { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal KitComponentQty { get; set; }
        public decimal KitComponentUnitPrice { get; set; }
    }
    public class PoAddKitProjectUpdateDto
    {
        public string? PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? Description { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? DrawingRevNo { get; set; }
        public PoPartType PartType { get; set; }
        public string? UOM { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal KitComponentQty { get; set; }
        public decimal KitComponentUnitPrice { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public PoStatus PoAddKitProjectStatus { get; set; }
    }
}
