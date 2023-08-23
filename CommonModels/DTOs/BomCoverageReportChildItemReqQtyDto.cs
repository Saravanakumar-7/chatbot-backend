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

    public class OpenSalesCoverageReportDto
    {
        public string? ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal? OpenSOQty { get; set; }
        public decimal? Stock { get; set; }
        public decimal? OpenPoQty { get; set; }
        public decimal BalanceToOrder { get; set; }
        public decimal? TotalRequiredQty { get; set; }
        public string? Status { get; set; }
    }
}
