using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class FG_Weighted_AvgCost_Report
    {
        public string FGItemnumber { get; set; }
        public string FGChildItemnumber { get; set; }
        public string? DrawingRevNo { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string ManufacturerPartNo { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class Weighted_AvgCost_Report
    {
        public string Itemnumber { get; set; }
        public decimal Avg_cost { get; set; }
        public DateTime update_date { get; set; }
    }

    public class FG_Weighted_AvgCost_Report_withDate
    {
        public string FGItemnumber { get; set; }
        public string FGChildItemnumber { get; set; }
        public string? DrawingRevNo { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string ManufacturerPartNo { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class FG_Weighted_AvgCost_ReportDto
    {
        public string FGItemNumber { get; set; }
    }
}
