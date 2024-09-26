using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EnggBomSPReport
    {
        public string? PartType { get; set; }
        public string? MaterialGroup { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? manufacturepartnumber1 { get; set; }
        public string? manufacturepartnumber2 { get; set; }
        public string? manufacturepartnumber3 { get; set; }
        public string? manufacturepartnumber4 { get; set; }
        public string? manufacturepartnumber5 { get; set; }
        public string? childitemno { get; set; }
        public string? childdescription { get; set; }
        public string? Designator { get; set; }
        public decimal Quantity { get; set; }
        public string? FootPrint { get; set; }
        public string? Remarks { get; set; }
        public int? BOMId { get; set; }
    }
}
