using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ScopeOfSupplyDto
    {
        public int Id { get; set; }
        public string? ScopeOfSupplyName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ScopeOfSupplyPostDto
    {
        [Required(ErrorMessage = "ScopeOfSupplyName is required")]
        [StringLength(100, ErrorMessage = "ScopeOfSupplyName can't be longer than 100 characters")]
        public string? ScopeOfSupplyName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        

    }

    public class ScopeOfSupplyUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "ScopeOfSupplyName is required")]
        [StringLength(100, ErrorMessage = "ScopeOfSupplyName can't be longer than 100 characters")]
        public string? ScopeOfSupplyName { get; set; }
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
