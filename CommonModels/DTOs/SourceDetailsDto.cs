using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class SourceDetailsDto
    {
        public int Id { get; set; }
        public string? SourceDetailsName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class SourceDetailsPostDto
    {
        public string? SourceDetailsName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }
    }
    public class SourceDetailsUpdateDto
    {
        public int Id { get; set; }
        public string? SourceDetailsName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }
    }


}
