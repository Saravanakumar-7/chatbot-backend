using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Entities.Enums;

namespace Tips.Grin.Api.Entities
{
    public class KIT_GRINParts
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        [Required]
        [Precision(18, 3)]
        public decimal? Qty { get; set; }
        [Required]
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }
        [Required]
        public string MftrItemNumber { get; set; }
        [Required]
        public string ManufactureBatchNumber { get; set; }
        public bool IsKIT_IqcCompleted { get; set; }
        public bool IsKIT_BinningCompleted { get; set; }
        [Required]
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal POOrderQty { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal POBalancedQty { get; set; }
        [Required]
        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
       
        [Precision(13, 3)]
        public decimal RejectReturnQty { get; set; }
        [Required]
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
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? SerialNo { get; set; }
        public int KIT_GRINId { get; set; }
        public KIT_GRIN? KIT_GRIN { get; set; }
        public List<KIT_GRIN_ProjectNumbers> KIT_GRIN_ProjectNumbers { get; set; }
    }
}
