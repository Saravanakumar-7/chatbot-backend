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
    public class ForecastLpCostingItem
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        [Precision(13, 3)]
        public decimal? TotalCost { get; set; }
        [Precision(13, 3)]
        public decimal? MaterialCost { get; set; }
        [Precision(13, 3)]
        public decimal? MarkUpForMaterial { get; set; }    
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int ForecastLpCostingId { get; set; }
        public ForecastLpCosting? ForecastLpCosting { get; set; }
        public List<ForecastLpCostingProcess>? ForecastLpCostingProcesses { get; set; }
        public List<ForecastLPCostingNREConsumable>? ForecastLPCostingNREConsumables { get; set; }
        public List<ForecastLpCostingOtherCharges>? ForecastLpCostingOtherCharges { get; set; }
    }
}
