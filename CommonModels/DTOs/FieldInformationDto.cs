using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class FieldInformationDto
    {
        public int Id { get; set; }
        public string FieldId { get; set; }
        public string? MenuName { get; set; }
        public string? TabName { get; set; }
        public string? FieldInfo { get; set; }
        public bool IsActive { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
