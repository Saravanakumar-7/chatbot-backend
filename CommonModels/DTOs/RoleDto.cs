using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class RoleDto
    {
      public int Id { get; set; }
      public string? RoleName { get; set; }
      public string Description { get; set; }
      public bool IsActive { get; set; } = true;
      public string Unit { get; set; }
      public string? CreatedBy { get; set; }
      public DateTime? CreatedOn { get; set; }

    }
    public class RolePostDto
    {
        public string? RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }
    public class RoleUpdateDto
    {
        public string? RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }
}
