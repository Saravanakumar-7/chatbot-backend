using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class GST_PercentageDto
    {
        public int? Id { get; set; }
        public string GSTPercentage { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class GST_PercentageDtoPost
    {
        [Required(ErrorMessage = "GSTPercentage is required")]
        [StringLength(100, ErrorMessage = "AuditFrequency can't be longer than 100 characters")]
        public string GSTPercentage { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
     
    }
    public class GST_PercentageDtoUpdate
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "GSTPercentage is required")]
        [StringLength(100, ErrorMessage = "GSTPercentage can't be longer than 100 characters")]
        public string GSTPercentage { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }
    }
}
