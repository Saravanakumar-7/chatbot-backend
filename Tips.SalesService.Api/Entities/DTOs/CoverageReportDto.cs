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
    public class CoverageReportDto
    {
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public decimal? BalanceForcastQty { get; set; }

    }
    public class GetCoverageReportDto
    {
        public string ItemNumber { get; set; }
        public string SalesOrderNumber { get; set; }
        public string Description { get; set; }
        public decimal ForecastQty { get; set; }
        public decimal FGStock { get; set; }

        public decimal? BalanceForcastQty { get; set; }
        public string Child { get; set; }
        public string ChildPartDescription { get; set; }

        //public int PartType { get; set; }
        public decimal QtyPerUnit { get; set; }
        public decimal ChildRequiredQty { get; set; }
        public decimal TotalChildReqQty { get; set; }

    }
    public class OpenPurchaseOrderDto
    {
        public string? ItemNumber { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal Qty { get; set; }
        public string? PONumber { get; set; }

    }

    public class ChildItemStockWithWipDto
    {
        public string PartNumber { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal WipQuantity { get; set; }

    }

    public class OpenPoQuantityDto
    {
        public string? ItemNumber { get; set; }
        public decimal OpenPoQty { get; set; }
    }

    public class CoverageReportDtoForChildItem
    {
        public string? ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? Stock { get; set; }
        public decimal? WipQty { get; set; }
        public decimal? OpenPoQty { get; set; }
        public decimal? BalanceToOrder { get; set; }
        
    }
    public class CoverageReportByProjectNumberDtoForChildItem
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? Stock { get; set; }
        public decimal? WipQty { get; set; }
        public decimal? OpenPoQty { get; set; }
        public decimal? BalanceToOrder { get; set; }

    }

}
