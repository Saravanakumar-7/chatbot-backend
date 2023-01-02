using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EnggBomGroupDto
    {
        public int Id { get; set; }
        public string BomGroupName { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class EnggBomGroupDtoPost
    {
        public string BomGroupName { get; set; }
        public string Remarks { get; set; }
     
    }
    public class EnggBomGroupDtoUpdate
    {
        public int Id { get; set; }
        public string BomGroupName { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
