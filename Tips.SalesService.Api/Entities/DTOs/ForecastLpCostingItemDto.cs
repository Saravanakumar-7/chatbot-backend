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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ForecastLpCostingProcessDto>? forecastLpCostingProcesses { get; set; }
        public List<ForecastLpCostingNREConsumableDto>? forecastLpCostingNREConsumables { get; set; }
        public List<ForecastLpCostingOtherChargesDto>? forecastLpCostingOtherCharges { get; set; }

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
        public string Unit { get; set; }
        public List<ForecastLPCostingProcessDtoPost>? forecastLPCostingProcesses { get; set; }
        public List<ForecastLPCostingNREConsumableDtoPost>? forecastLPCostingNREConsumables { get; set; }
        public List<ForecastLPCostingOtherChargesDtoPost>? forecastLPCostingOtherCharges { get; set; }

    }
    public class ForecastLPCostingItemDtoUpdate
    {
        public int Id { get; set; }
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
        public string Unit { get; set; }
        public List<ForecastLPCostingProcessDtoUpdate>? forecastLPCostingProcesses { get; set; }
        public List<ForecastLPCostingNREConsumableDtoUpdate>? forecastLPCostingNREConsumables { get; set; }
        public List<ForecastLPCostingOtherChargesDtoUpdate>? forecastLPCostingOthers { get; set; }
    }
}
