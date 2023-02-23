using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class SegmentDto
    {
        public int Id { get; set; }
        [Required]
        public string SegmentName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class SegmentDtoPost
    {
        [Required]
        public string SegmentName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; }
      
    }
    public class SegmentDtoUpdate
    {
        public int Id { get; set; }
        [Required]
        public string SegmentName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; }
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }
    }

}
