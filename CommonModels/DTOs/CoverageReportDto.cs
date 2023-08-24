using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CoverageReportDto
    {
        public string? ChildItemNumber { get; set; }
        public decimal? OpenSOQty { get; set; }
        public decimal? Stock { get; set; }
        public decimal? OpenPoQty { get; set; }
        public decimal? BalanceToOrderQtyChild { get; set; }
        public decimal TotalRequiredQty { get; set; } 
    }
}
