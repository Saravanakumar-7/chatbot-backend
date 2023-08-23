using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities.DTOs
{
    public class BomCoverageReportChildItemReqQtyDto
    {
        public string ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal RequiredQty { get; set; }
    }
}
