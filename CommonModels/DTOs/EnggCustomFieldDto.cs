using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EnggCustomFieldDto
    {
        public int Id { get; set; }
        public string BOMGroupName { get; set; }
        public string LabelName { get; set; }
        public string Type { get; set; }
        public string MaxLength { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class EnggCustomFieldDtoPost
    {
        public string BOMGroupName { get; set; }
        public string LabelName { get; set; }
        public string Type { get; set; }
        public string MaxLength { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class EnggCustomFieldDtoUpdate
    {
        public int Id { get; set; }
        public string BOMGroupName { get; set; }
        public string LabelName { get; set; }
        public string Type { get; set; }
        public string MaxLength { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
