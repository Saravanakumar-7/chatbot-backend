using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ForecastLpCostingItemDto
    {
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

        public List<ForecastLpCostingProcessDto>? ForecastLpCostingProcesses { get; set; }
        public List<ForecastLpCostingNREConsumableDto>? ForecastLpCostingNREConsumables { get; set; }
        public List<ForecastLpCostingOtherChargesDto>? ForecastLpCostingOtherCharges { get; set; }

    }
    public class ForecastLPCostingItemDtoPost
    {

        [StringLength(500, ErrorMessage = "ItemNumber can't be longer than 500 characters")]

        public string? ItemNumber { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]

        public string? Description { get; set; }
        [Precision(13, 3)]
        public decimal? TotalCost { get; set; }
        [Precision(13, 3)]
        public decimal? MaterialCost { get; set; }
        [Precision(13, 3)]
        public decimal? MarkUpForMaterial { get; set; }
    
        public List<ForecastLPCostingProcessDtoPost>? ForecastLPCostingProcesses { get; set; }
        public List<ForecastLPCostingNREConsumableDtoPost>? ForecastLPCostingNREConsumables { get; set; }
        public List<ForecastLPCostingOtherChargesDtoPost>? ForecastLPCostingOtherCharges { get; set; }

    }
    public class ForecastLPCostingItemDtoUpdate
    {
     
        [StringLength(500, ErrorMessage = "ItemNumber can't be longer than 500 characters")]

        public string? ItemNumber { get; set; }
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]

        public string? Description { get; set; }
        [Precision(13, 3)]
        public decimal? TotalCost { get; set; }
        [Precision(13, 3)]
        public decimal? MaterialCost { get; set; }
        [Precision(13, 3)]
        public decimal? MarkUpForMaterial { get; set; }
     
        public List<ForecastLPCostingProcessDtoUpdate>? ForecastLPCostingProcesses { get; set; }
        public List<ForecastLPCostingNREConsumableDtoUpdate>? ForecastLPCostingNREConsumables { get; set; }
        public List<ForecastLPCostingOtherChargesDtoUpdate>? ForecastLPCostingOthers { get; set; }
    }
}
