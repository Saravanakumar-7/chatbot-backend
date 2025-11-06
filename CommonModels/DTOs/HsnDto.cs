using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class HsnDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Hsn is required")]
        public string? hsn { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        public decimal? Rate { get; set; }

        [Required(ErrorMessage = "IsActive flag is required")]
        public bool? IsActive { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }

    public class HsnPostDto
    {

        [Required(ErrorMessage = "Hsn is required")]
        public string? hsn { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        public decimal? Rate { get; set; }

        [Required(ErrorMessage = "IsActive flag is required")]
        public bool? IsActive { get; set; }

    }
    public class HsnUpdateDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Hsn is required")]
        public string? hsn { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        public decimal? Rate { get; set; }

        [Required(ErrorMessage = "IsActive flag is required")]
        public bool? IsActive { get; set; }
       
    }
}
