using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class UnitDto
    {
        public int? Id { get; set; }
        public string UnitName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class UnitPostDto
    {
        public string UnitName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class UnitUpdateDto
    {
        public int? Id { get; set; }
        public string UnitName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

}
