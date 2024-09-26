using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class MaterialRequestItems
    {
        [Key]
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public PartType PartType { get; set; }
        public string? Stock { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        public IssuedStatus IssueStatus { get; set; }

        [Precision(13, 3)]
        public decimal? RequiredQty { get; set; }

        public int? MaterialRequestId { get; set; }
        public MaterialRequests? MaterialRequest { get; set; }

        public List<MRStockDetails>? MRStockDetails { get; set; }

    }
}
