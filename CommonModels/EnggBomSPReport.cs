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
        public int? BOMId {  get; set; }
        public PartType? PartType { get; set; }
        public string? MaterialGroup { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? ManufacturePartNumber { get; set; }
        public string? ChildItemNo { get; set; }
        public string? ChildDescription { get; set; }
        public string? Designator { get; set; }
        public decimal Quantity { get; set; }
        public string? FootPrint { get; set; }
        public string? Remarks { get; set; }
    }
}
