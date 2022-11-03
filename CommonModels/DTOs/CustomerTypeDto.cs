using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public  class CustomerTypeDto
    {
        public int? Id { get; set; }
        public string? CustomerTypeName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }


    public class CustomerTypeDtoPost
    {
        [Required(ErrorMessage = "CustomerType is required")]
        [StringLength(100, ErrorMessage = "CustomerType can't be longer than 100 characters")]
        public string? CustomerTypeName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
