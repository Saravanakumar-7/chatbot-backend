using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public class KIT_GRINPartsDto {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        [Precision(18, 3)]
        public decimal Qty { get; set; }
        public string ItemDescription { get; set; }
        public string PONumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string ManufactureBatchNumber { get; set; }
        public bool IsKIT_IqcCompleted { get; set; }
        public bool IsKIT_BinningCompleted { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal POOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal POBalancedQty { get; set; }
        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectReturnQty { get; set; }
        public string UOM { get; set; }
        public string? UOC { get; set; }
        public string? Remarks { get; set; }
        [DefaultValue(0)]
        public GrinStatus Status { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public bool IsCOCUploaded { get; set; } = false;
        public string? CoCUpload { get; set; }
        [Precision(13, 3)]
        public decimal? SGST { get; set; }
        [Precision(13, 3)]
        public decimal? IGST { get; set; }
        [Precision(13, 3)]
        public decimal? CGST { get; set; }
        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }        
        public string? SerialNo { get; set; }
        public string? DrawingNo { get; set; }
        [NotMapped]
        public string? DocRet { get; set; }
        [NotMapped]
        public string? RevNo { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool IsCocRequired { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool IsRohsItem { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool IsShelfLife { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool IsReachItem { get; set; }
        public int KIT_GRINId { get; set; }        
        public List<KIT_GRIN_ProjectNumbersDto> KIT_GRIN_ProjectNumbers { get; set; }
    }
    public class KIT_GRINPartsUpdateDto
    {
        public int Id { get; set; }
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
        public List<KIT_GRIN_ProjectNumbersUpdateDto> KIT_GRIN_ProjectNumbers { get; set; }
    }
}
