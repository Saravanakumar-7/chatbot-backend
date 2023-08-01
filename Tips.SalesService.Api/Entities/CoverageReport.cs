using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class CoverageReport
    {
        public string Id { get; set; }

        public string ItemNumber { get; set; }
        public string SalesOrderNumber { get; set; }
        public string Description { get; set; }
        public decimal ForecastQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal BalanceQtyToOrder { get; set; }        
        public decimal OpenPOQty { get; set; }        

        public decimal BalanceForcastQty { get; set; }
        public string Child { get; set; }
        public string ChildPartDescription { get; set; }

        //public int PartType { get; set; }
        public decimal QtyPerUnit { get; set; }
        public decimal ChildRequiredQty { get; set; }
        public decimal TotalChildReqQty { get; set; }

    }
}
