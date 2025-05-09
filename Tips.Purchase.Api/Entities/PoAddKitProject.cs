using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PoAddKitProject
    {
        [Key]
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? Description { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? DrawingRevNo { get; set; }
        public PoPartType PartType { get; set; }
        public string? UOM { get; set; }
        public string? ProjectNumber { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }
        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }
        public PoStatus PoAddKitProjectStatus { get; set; }
      
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PoAddProjectId { get; set; }
        public PoAddProject? PoAddProject { get; set; }
    }
}
