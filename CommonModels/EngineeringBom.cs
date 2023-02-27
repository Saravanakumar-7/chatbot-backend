using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EngineeringBom
    {
        [Key]
        public int Id { get; set; }
       public string ReleaseFor { get; set; }
       public string ItemNumber { get; set; }
       public decimal ReleaseVersion { get; set; }
       public string ReleaseNote { get; set; }
        [DefaultValue(false)]
        public bool IsReleaseCompleted { get; set; }
        [DefaultValue(false)]
        public bool IsReleaseCostCompleted { get; set; }
        [DefaultValue(false)]
        public bool IsReleaseProductCompleted { get; set; } 
        public string? CreatedBy { get; set; }
       public DateTime? CreatedOn { get; set; }
       public string? LastModifiedBy { get; set; }
       public DateTime? LastModifiedOn { get; set; }
    }
    
}
