using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class CategoryDtoPost
    {
        [Required]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }

    }
    public class CategoryDtoUpdate
    {
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }

    }
}