using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Ranges
    {
        [Key]
        public int Id {  get; set; }
        public decimal RangeFrom { get; set; }
        public decimal RangeTo { get; set; }
        public bool Approval1 { get; set; }
        public bool Approval2 { get; set; }
        public bool Approval3 { get; set; }
        public bool Approval4 { get; set; }
        public int ApprovalRangesId { get; set; }
        public ApprovalRanges ApprovalRanges { get; set; }
    }
}
