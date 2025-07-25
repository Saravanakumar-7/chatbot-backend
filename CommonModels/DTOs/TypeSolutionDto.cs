using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class TypeSolutionDto
    {
        public int Id { get; set; }
        public string TypeSolutionName { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public string? Remarks { get; set; }

        public bool? IsActive { get; set; }

        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class TypeSolutionPostDto
    {
        [Required(ErrorMessage = "TypeSolutionName is required")]
        public string TypeSolutionName { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public string? Remarks { get; set; }

        public bool? IsActive { get; set; }

    } 
    public class TypeSolutionUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "TypeSolutionName is required")]
        public string TypeSolutionName { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public string? Remarks { get; set; }

        public bool? IsActive { get; set; }

        public string? Unit { get; set; }
    }

}