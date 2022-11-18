using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VolumeUomDto
    {
        public int Id { get; set; }
        public string? VolumeUomName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class VolumeUomPostDto
    {
        [Required(ErrorMessage = "VolumeUOM is required")]
        [StringLength(100, ErrorMessage = "VolumeUOM can't be longer than 100 characters")]
        public string? VolumeUomName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }

    }

    public class VolumeUomUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "VolumeUOM is required")]
        [StringLength(100, ErrorMessage = "VolumeUOM can't be longer than 100 characters")]
        public string? VolumeUomName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }
    }
}
