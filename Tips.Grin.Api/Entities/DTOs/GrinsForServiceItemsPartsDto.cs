using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class GrinsForServiceItemsPartsDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }
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
        public bool IsIqcForServiceItemsCompleted { get; set; }
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
        public decimal? AverageCost { get; set; }
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
        public List<GrinsForServiceItemsProjectNumbersDto>? GrinsForServiceItemsProjectNumbers { get; set; }
    }
    public class GrinsForServiceItemsPartsPostDto
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
        public List<GrinsForServiceItemsProjectNumbersPostDto>? GrinsForServiceItemsProjectNumbers { get; set; }
    }
    public class GrinsForServiceItemsPartsUpdateDto
    {
        [Required(ErrorMessage = "ItemDescription is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        [Precision(18, 3)]
        public decimal? Qty { get; set; }
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
        public decimal? AverageCost { get; set; }
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
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? SerialNo { get; set; }
        public List<GrinsForServiceItemsProjectNumbersUpdateDto>? GrinsForServiceItemsProjectNumbers { get; set; }
    }
}
