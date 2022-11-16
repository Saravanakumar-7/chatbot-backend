using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class GrinPartsDto
    {
        public int Id { get; set; }

        public string ItemNODescription { get; set; }

        public string MftrItemNumber { get; set; }

        public string ProjectNumber { get; set; }

        public string ManufactureBatchNumber { get; set; }

        public decimal UnitPrice { get; set; }

        public int POOrderQuantity { get; set; }

        public int POBalancedQuantity { get; set; }

        public decimal POUnitPrice { get; set; }

        public int Quantity { get; set; }

        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string COCUpload { get; set; }
        public decimal? SGST { get; set; }

        public decimal? IGST { get; set; }

        public decimal? CGST { get; set; }

        public decimal? UTGST { get; set; }


        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class GrinPartsPostDto
    {
        [Required(ErrorMessage = "ItemNODescription is required")]
        [StringLength(500, ErrorMessage = "ItemNODescription can't be longer than 500 characters")]

        public string ItemNODescription { get; set; }

        [Required(ErrorMessage = "MftrItemNumber is required")]
         public string MftrItemNumber { get; set; }

        [Required(ErrorMessage = "ProjectNumber is required")]
        public string ProjectNumber { get; set; }

        [Required(ErrorMessage = "ManufactureBatchNumber is required")]
        public string ManufactureBatchNumber { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]

        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "POOrderQuantity is required")]

        public int POOrderQuantity { get; set; }

        [Required(ErrorMessage = "POBalancedQuantity is required")]

        public int POBalancedQuantity { get; set; }

        [Required(ErrorMessage = "POUnitPrice is required")]
        public decimal POUnitPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? COCUpload { get; set; }
        public decimal? SGST { get; set; }

        public decimal? IGST { get; set; }

        public decimal? CGST { get; set; }

        public decimal? UTGST { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class GrinPartsUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ItemNODescription is required")]
        [StringLength(500, ErrorMessage = "ItemNODescription can't be longer than 500 characters")]

        public string ItemNODescription { get; set; }

        [Required(ErrorMessage = "MftrItemNumber is required")]
        public string MftrItemNumber { get; set; }

        [Required(ErrorMessage = "ProjectNumber is required")]
        public string ProjectNumber { get; set; }

        [Required(ErrorMessage = "ManufactureBatchNumber is required")]
        public string ManufactureBatchNumber { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]

        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "POOrderQuantity is required")]

        public int POOrderQuantity { get; set; }

        [Required(ErrorMessage = "POBalancedQuantity is required")]

        public int POBalancedQuantity { get; set; }

        [Required(ErrorMessage = "POUnitPrice is required")]
        public decimal POUnitPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? COCUpload { get; set; }
        public decimal? SGST { get; set; }

        public decimal? IGST { get; set; }

        public decimal? CGST { get; set; }

        public decimal? UTGST { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}