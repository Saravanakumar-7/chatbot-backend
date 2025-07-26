using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class AdditionalChargesDto
    {
        public int? Id { get; set; }

        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }
        public decimal? AddtionalChargesValueAmount { get; set; }
        public decimal? IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? SGST { get; set; }

        public bool ActiveStatus { get; set; } = true;
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class AdditionalChargesPostDto
    {
        [Required(ErrorMessage = "AdditionalChargesLabelName is required")]
        public string? AdditionalChargesLabelName { get; set; }
        [Required(ErrorMessage = "AddtionalChargesValueType is required")]
        public string? AddtionalChargesValueType { get; set; }
        public decimal? AddtionalChargesValueAmount { get; set; }

        public bool ActiveStatus { get; set; } = true;
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public decimal? IGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? SGST { get; set; }
    }
    public class AdditionalChargesUpdateDto
    {
        [Required]
        public int? Id { get; set; }
        [Required(ErrorMessage = "AdditionalChargesLabelName is required")]
        public string? AdditionalChargesLabelName { get; set; }

        [Required(ErrorMessage = "AddtionalChargesValueType is required")]
        public string? AddtionalChargesValueType { get; set; }
        public decimal? AddtionalChargesValueAmount { get; set; }
        public decimal? IGST { get; set; }
        public decimal? CGST { get; set; }

        public bool ActiveStatus { get; set; } = true;

        public decimal? UTGST { get; set; }
        public decimal? SGST { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
    }


}
    
