using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CostingMethodDto
    {
        public int? Id { get; set; }
        public string CostingMethodName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class CostingMethodDtoPost
    {
        [Required(ErrorMessage = "CostingMethod is required")]
        [StringLength(100, ErrorMessage = "CostingMethod can't be longer than 100 characters")]
        public string? CostingMethodName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
    public class CostingMethodDtoUpdate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "CostingMethod is required")]
        [StringLength(100, ErrorMessage = "CostingMethod can't be longer than 100 characters")]
        public string? CostingMethodName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
