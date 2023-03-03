using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class TypeOfHomeDto
    {
        public int Id { get; set; }
        public string? TypeofHome { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class TypeOfHomePostDto
    {
        [Required(ErrorMessage = "TypeOfHome is required")]
        [StringLength(100, ErrorMessage = "TypeOfHome can't be longer than 100 characters")]
        public string? TypeofHome { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }
    }
    public class TypeOfHomeUpdateDto
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "TypeOfHome is required")]
        [StringLength(100, ErrorMessage = "TypeOfHome can't be longer than 100 characters")]
        public string? TypeofHome { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }
    }

}
