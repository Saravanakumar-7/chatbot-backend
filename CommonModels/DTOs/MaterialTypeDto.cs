using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class MaterialTypeDto
    {
        public int? Id { get; set; }
        public string? MaterialTypeName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }


    public class MaterialTypeDtoPost
    {
        [Required(ErrorMessage = "MaterialType is required")]
        [StringLength(100, ErrorMessage = "MaterialType can't be longer than 100 characters")]
        public string? MaterialTypeName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
    