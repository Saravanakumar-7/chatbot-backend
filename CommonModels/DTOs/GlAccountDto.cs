using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class GlAccountDto
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Group { get; set; }
        [Required(ErrorMessage = "IsActive is required")]
        public bool? IsActive { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string? Unit { get; set; }
        [Required(ErrorMessage = "CreatedBy is CreatedBy")]
        public string? CreatedBy { get; set; }
        [Required(ErrorMessage = "CreatedOn is required")]
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class GlAccountPostDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Group { get; set; }
        [Required(ErrorMessage = "IsActive is required")]
        public bool? IsActive { get; set; }
       
    }
    public class GlAccountUpdateDto
    {
        [Required]
        public int? Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Group { get; set; }
        [Required(ErrorMessage = "IsActive is required")]
        public bool? IsActive { get; set; }

    }
}
