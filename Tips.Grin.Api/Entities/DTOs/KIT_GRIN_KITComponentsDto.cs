using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_GRIN_KITComponentsPostDto
    {
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public PoPartType PartType { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
    }
    public class KIT_GRIN_KITComponentscalculationofAvgcost
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public PoPartType PartType { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal? AverageCost { get; set; }
        [NotMapped]
        public decimal? EPwithTax { get; set; }
        [NotMapped]
        public decimal? EPforSingleQty { get; set; }
    }
}
