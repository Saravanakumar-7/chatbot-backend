using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class DeliveryTermGetDto
    {
            public int Id { get; set; }
            public string? DeliveryTermName { get; set; }
            public string? Description { get; set; }
            public string? Remarks { get; set; }
            public bool IsActive { get; set; } = true;
            public string? CreatedBy { get; set; }
            public DateTime? CreatedOn { get; set; }
            public string? LastModifiedBy { get; set; }
            public DateTime? LastModifiedOn { get; set; }
        
    }

    public class DeliveryTermPostDto
    {
        [Required(ErrorMessage = "CustomerType is required")]
        [StringLength(100, ErrorMessage = "CustomerType can't be longer than 100 characters")]
        public string? DeliveryTermName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")]
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        
    }

    public class DeliveryTermUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "DeliveryTerm is required")]
        [StringLength(100, ErrorMessage = "DeliveryTerm can't be longer than 100 characters")]
        public string? DeliveryTermName { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        [StringLength(500, ErrorMessage = "Remarks can't be longer than 500 characters")] 
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
