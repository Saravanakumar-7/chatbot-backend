using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Tips.Grin.Api.Entities.Enums;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_GRIN_KITComponentsPostDto
    {
        public string PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string Description { get; set; }
        public string? DrawingRevNo { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentBOMQty { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
    }
    public class KIT_GRIN_KITComponentscalculationofAvgcost
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string Description { get; set; }
        public decimal KitRevisionNo { get; set; }
        public string? DrawingRevNo { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentBOMQty { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal BinnedQty { get; set; } = 0;
        public string LotNumber { get; set; }
        public int KIT_GRIN_ProjectNumbersId { get; set; }
        [Precision(13, 3)]
        public decimal? AverageCost { get; set; }
        [NotMapped]
        public decimal? EPwithTax { get; set; }
        [NotMapped]
        public decimal? EPforSingleQty { get; set; }
    }
    public class KIT_GRIN_KITComponentsDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string Description { get; set; }
        public decimal KitRevisionNo { get; set; }
        public string? DrawingRevNo { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentBOMQty { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal BinnedQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal? AverageCost { get; set; }
        public string LotNumber { get; set; }
        public int KIT_GRIN_ProjectNumbersId { get; set; }
    }
    public class KIT_GRIN_KITComponentsUpdateDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string Description { get; set; }
        public string? DrawingRevNo { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentBOMQty { get; set; }
        [Precision(13, 3)]
        public decimal KitComponentQty { get; set; }
        [Precision(18, 3)]
        public decimal KitComponentUnitPrice { get; set; }
    }
}
