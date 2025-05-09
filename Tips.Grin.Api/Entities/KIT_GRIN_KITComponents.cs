using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Grin.Api.Entities
{
    public class KIT_GRIN_KITComponents
    {
        [Key]
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public string? MftrItemNumbers { get; set; }
        public PoPartType PartType { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal? AverageCost { get; set; }
        public string? LotNumber { get; set; }
        public int KIT_GRIN_ProjectNumbersId { get; set; }
        public KIT_GRIN_ProjectNumbers? KIT_GRIN_ProjectNumbers { get; set; }

    }
}
