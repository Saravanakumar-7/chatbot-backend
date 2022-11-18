using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class AuditFrequencyDto
    {
        public int? Id { get; set; }
        public string AuditFrequencyName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class AuditFrequencyDtoPost
    {
        [Required(ErrorMessage = "AuditFrequency is required")]
        [StringLength(100, ErrorMessage = "AuditFrequency can't be longer than 100 characters")]
        public string AuditFrequencyName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]

        public string Unit { get; set; }

    }
    public class AuditFrequencyDtoUpdate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "AuditFrequency is required")]
        [StringLength(100, ErrorMessage = "AuditFrequency can't be longer than 100 characters")]
        public string AuditFrequency { get; set; }
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
