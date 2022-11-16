using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VendorCategoryDto
    {
        public int Id { get; set; }
        public string? VendorCategoryName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
         

    }
    public class VendorCategoryPostDto
    {
        [Required(ErrorMessage = "VendorCategoryName is required")]
        [StringLength(100, ErrorMessage = "VendorCategoryName can't be longer than 100 characters")]
        public string? VendorCategoryName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class VendorCategoryUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "VendorCategoryName is required")]
        [StringLength(100, ErrorMessage = "VendorCategoryName can't be longer than 100 characters")]
        public string? VendorCategoryName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
