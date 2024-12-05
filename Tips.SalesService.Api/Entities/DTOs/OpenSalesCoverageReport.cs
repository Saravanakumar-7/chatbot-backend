using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class OpenSalesCoverageReport
    {
        public string? ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal? OpenSOQty { get; set; }
        public decimal? OpenRetailSOQty { get; set; }
        public decimal? MSL { get; set; }
        public string? UOM { get; set; }
        public decimal Stock { get; set; }
        public decimal OpenPoQty { get; set; }
        public decimal BalanceToOrder { get; set; }
        public decimal? TotalRequiredQty { get; set; }
        //public string? Status { get; set; }
    }
    public class OpenSalesCoverageReportByProjectNumber
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? MaterialGroup { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOM { get; set; }
        public PartType PartType { get; set; }
        public decimal? OpenSOQty { get; set; }
        public decimal Stock { get; set; }
        public decimal OpenPoQty { get; set; }
        public decimal BalanceToOrder { get; set; }
        public decimal? TotalRequiredQty { get; set; }
        //public string? Status { get; set; }
    }
}
