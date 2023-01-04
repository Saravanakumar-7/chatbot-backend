using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class GrinPartsDto
    {
        public int Id { get; set; } 
        public string ItemDescription { get; set; }
        public string? ItemNumber { get; set; }

        [Precision(18, 3)]
        public decimal? Qty { get; set; }
        public string? PONumber { get; set; }

        public string MftrItemNumber { get; set; }

        public string? ProjectNumber { get; set; }

        public string ManufactureBatchNumber { get; set; }

        [Precision(18,3)]
        public decimal UnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal POOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal POBalancedQty { get; set; }

        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }

       

        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? COCUpload { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]

        public decimal? IGST { get; set; }

        [Precision(13, 3)]

        public decimal? CGST { get; set; }

        [Precision(13, 3)]

        public decimal? UTGST { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ProjectNumbersDto>? ProjectNumbers { get; set; }


    }
    public class GrinPartsPostDto
    {
        

        [Required(ErrorMessage = "ItemDescription is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }

        public string? ItemNumber { get; set; }

        [Precision(18, 3)]
        public decimal? Qty { get; set; }


        [Required(ErrorMessage = "MftrItemNumber is required")]
        public string MftrItemNumber { get; set; }

        [Required(ErrorMessage = "ProjectNumber is required")]
        public string? ProjectNumber { get; set; }

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

        

        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? COCUpload { get; set; }
        public decimal? SGST { get; set; }

        public decimal? IGST { get; set; }

        public decimal? CGST { get; set; }

        public decimal? UTGST { get; set; }
        


        public List<ProjectNumbersDtoPost>? ProjectNumbers { get; set; }


    }
    public class GrinPartsUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ItemDescription is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }

        public string? ItemNumber { get; set; }

        [Precision(18, 3)]
        public decimal? Qty { get; set; }

        [Required(ErrorMessage = "MftrItemNumber is required")]
        public string MftrItemNumber { get; set; }

        [Required(ErrorMessage = "ProjectNumber is required")]
        public string? ProjectNumber { get; set; }

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


        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? COCUpload { get; set; }
        public decimal? SGST { get; set; }

        public decimal? IGST { get; set; }

        public decimal? CGST { get; set; }

        public decimal? UTGST { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        
        public List<ProjectNumbersDtoUpdate>? ProjectNumbers { get; set; }
    }
}