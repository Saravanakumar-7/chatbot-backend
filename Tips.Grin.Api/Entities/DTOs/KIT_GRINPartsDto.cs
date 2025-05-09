using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_GRINPartsPostDto
    {
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }
        public int PoItemId { get; set; }
        public string? ItemNumber { get; set; }

        [Precision(18, 3)]
        public decimal? Qty { get; set; }
        public PartType ItemType { get; set; }

        [Required(ErrorMessage = "MftrItemNumber is required")]
        public string MftrItemNumber { get; set; }
        [Required(ErrorMessage = "ManufactureBatchNumber is required")]
        public string ManufactureBatchNumber { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "POOrderQty is required")]
        [Precision(13, 3)]
        public decimal POOrderQty { get; set; }

        [Required(ErrorMessage = "POBalancedQty is required")]
        [Precision(13, 3)]
        public decimal POBalancedQty { get; set; }

        [Required(ErrorMessage = "POUnitPrice is required")]
        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal? WeightedAverage { get; set; }

        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }
        public string? UOC { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? Remarks { get; set; }
        public string? COCUpload { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        public string? SerialNo { get; set; }
        public List<KIT_GRIN_ProjectNumbersPostDto> KIT_GRIN_ProjectNumbers { get; set; }
    }
}
