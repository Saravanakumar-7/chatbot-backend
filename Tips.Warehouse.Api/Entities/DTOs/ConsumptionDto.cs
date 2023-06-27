using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ConsumptionDto
    {
        public string ItemNumber { get; set; }
        public string SalesOrderNumber { get; set; }
        public string Description { get; set; }
        public decimal ForecastQty { get; set; }
        public decimal FGStock { get; set; }

        public decimal BalanceForecastQty { get; set; }
        public string Child { get; set; }
        public string ChildPartDescription { get; set; }

        //public int PartType { get; set; }
        public decimal QtyPerUnit { get; set; }
        public decimal ChildRequiredQty { get; set; }
        public decimal TotalChildReqQty { get; set; }


    }
}
