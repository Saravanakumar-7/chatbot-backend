using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApprovalRanges
    {
        [Key]
        public int Id { get; set; }
        public string ProcurementName { get; set; }
        public string? Description { get; set; }
        public int Version { get; set; }
        public List<Ranges> Ranges { get; set; }
        public string? Remarks { get; set; } 
        public string Unit { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }        
    }
}
