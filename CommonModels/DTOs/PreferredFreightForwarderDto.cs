using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class PreferredFreightForwarderDto
    {
        public int? Id { get; set; }
        public string PreferredFreightforwarder { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class PreferredFreightForwarderDtoPost
    {
        [Required(ErrorMessage = "PreferredFreightForwarder is required")]
        [StringLength(100, ErrorMessage = "PreferredFreightForwarder can't be longer than 100 characters")]
        public string PreferredFreightforwarder { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
    public class PreferredFreightForwarderDtoUpdate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "PreferredFreightForwarder is required")]
        [StringLength(100, ErrorMessage = "PreferredFreightForwarder can't be longer than 100 characters")]
        public string PreferredFreightforwarder { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
