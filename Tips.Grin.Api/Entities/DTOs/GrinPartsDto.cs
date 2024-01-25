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
    public class GrinPartsDto
    {
        public int Id { get; set; }

        [Required]

        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }
        public int GrinPartId { get; set; }

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
        public decimal? WeightedAverage { get; set; }
        [Required]
        public string UOM { get; set; }
        public string? UOC { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? COCUpload { get; set; }
        public string? SerialNo { get; set; }
        public bool IsIqcCompleted { get; set; }

        public bool IsBinningCompleted { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        [NotMapped]
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
        public string? Remarks { get; set; }
        [NotMapped]
        public List<DocumentUpload>? FileUpload { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ProjectNumbersDto>? ProjectNumbers { get; set; }
       

    }
    public class GrinPartscalculationofAvgcost
    {        
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }
        public PartType ItemType { get; set; }
        public decimal? Qty { get; set; }
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string ManufactureBatchNumber { get; set; }
        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal POOrderQty { get; set; }
        public decimal POBalancedQty { get; set; }
        public decimal POUnitPrice { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        [NotMapped]
        public decimal? EPwithTax { get; set; }
        [NotMapped]
        public decimal? EPforSingleQty { get; set; }
        public decimal? AverageCost { get; set; }
        public string UOM { get; set; }
        public string? UOC { get; set; }
        public string? Remarks { get; set; }
        public GrinStatus Status { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public bool IsCOCUploaded { get; set; } = false;
        public string? CoCUpload { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public Grins? Grins { get; set; }
        public string? SerialNo { get; set; }
        public List<ProjectNumbers>? ProjectNumbers { get; set; }
    }
    public class GrinPartsPostDto
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

        //[Required(ErrorMessage = "ProjectNumbers is required")]
        //public string? ProjectNumbers { get; set; }

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

        //public string? COCUpload { get; set; }
        public decimal? SGST { get; set; }

        public decimal? IGST { get; set; }

        public decimal? CGST { get; set; }

        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        public string? SerialNo { get; set; }
        public List<ProjectNumbersDtoPost>? ProjectNumbers { get; set; }
    }
    public class GrinPartsUpdateDto
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

        //[Required(ErrorMessage = "ProjectNumbers is required")]
        //public string? ProjectNumbers { get; set; }

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
        //public string? COCUpload { get; set; }
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

        public List<ProjectNumbersDtoUpdate>? ProjectNumbers { get; set; }
    }
    //for this model code we are going to reduce balance qty in po while create grin parts
    public class GrinUpdateQtyDetailsDto
    {
        public string? ItemNumber { get; set; }
        public decimal? Qty { get; set; }
        public string? PONumber { get; set; }
    }
    public class GrinQtyPoStatusUpdateDto
    {
        public string? ItemNumber { get; set; }
        public decimal? Qty { get; set; }
        public string? PONumber { get; set; }
        public int PoItemId { get; set; }
    }
    public class GrinPartsItemMasterEnggDto
    {
        public int Id { get; set; }

        [Required]

        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }
        public int GrinPartId { get; set; }

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
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? SerialNo { get; set; }
        public string? COCUpload { get; set; }

        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }
        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
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
        //[NotMapped]
        //public List<DocumentUpload>? FileUpload { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ProjectNumbersDto>? ProjectNumbers { get; set; }
        //public string? DrawingNo { get; set; }
        //public string? DocRet { get; set; }
        //public string? RevNo { get; set; }
        //[DefaultValue(false)]
        //public bool IsCocRequired { get; set; }
        //[DefaultValue(false)]
        //public bool IsRohsItem { get; set; }
        //[DefaultValue(false)]
        //public bool IsShelfLife { get; set; }
        //[DefaultValue(false)]
        //public bool IsReachItem { get; set; }
        //public List<DocumentUpload>? FileUpload { get; set; }
    }
    public class GrinPartsReportDto
    {
        public int Id { get; set; }

        [Required]

        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }
        public string? GrinNumber { get; set; }
        public int GrinPartId { get; set; }

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
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? SerialNo { get; set; }
        public string? COCUpload { get; set; }


        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        public List<ProjectNumbersReportDto>? ProjectNumbers { get; set; }


    }
}