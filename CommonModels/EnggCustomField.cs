using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EnggCustomField
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
