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
    public class RfqLPCostingItemDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public string? CustomerItemNumber { get; set; }

        [Precision(13, 3)]
        public decimal? TotalCost { get; set; }
        [Precision(13, 3)]
        public decimal? MaterialCost { get; set; }
        [Precision(13, 3)]
        public decimal? MarkUpForMaterial { get; set; }

        public string? MarkUpForProcessSteps { get; set; }
        public int? NoOfProcess { get; set; }
        [Precision(13, 3)]
        public decimal? LandedPrice { get; set; }
        [Precision(13, 3)]
        public decimal? MOQCost { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqLPCostingProcessDto>? RfqLPCostingProcesses { get; set; }
        public List<RfqLPCostingNREConsumableDto>? RfqLPCostingNREConsumables { get; set; }
        public List<RfqLPCostingOtherChargesDto>? RfqLPCostingOtherCharges { get; set; }

    }
    public class RfqLPCostingItemDtoPost
    {
        public string? ItemNumber { get; set; }
       
        public string? CustomerItemNumber { get; set; }

        public string? Description { get; set; }
       
        public decimal? TotalCost { get; set; }
       
        public decimal? MaterialCost { get; set; }
          
        public decimal? MarkUpForMaterial { get; set; }
        public string? MarkUpForProcessSteps { get; set; }
        public int? NoOfProcess { get; set; }
        public decimal? LandedPrice { get; set; }
       
        public decimal? MOQCost { get; set; }

        public List<RfqLPCostingProcessDtoPost>? RfqLPCostingProcesses { get; set; }
        public List<RfqLPCostingNREConsumableDtoPost>? RfqLPCostingNREConsumables { get; set; }
        public List<RfqLPCostingOtherChargesDtoPost>? RfqLPCostingOtherCharges { get; set; }

    }
    public class RfqLPCostingItemDtoUpdate
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }       

        public string? CustomerItemNumber { get; set; }

        public string? Description { get; set; }
        [Precision(13, 3)]
        public decimal? TotalCost { get; set; }
        [Precision(13, 3)]
        public decimal? MaterialCost { get; set; }
        [Precision(13, 3)]
        public decimal? MarkUpForMaterial { get; set; }

        public string? MarkUpForProcessSteps { get; set; }
        public int? NoOfProcess { get; set; }
        [Precision(13, 3)]
        public decimal? LandedPrice { get; set; }
        [Precision(13, 3)]
        public decimal? MOQCost { get; set; }
        public List<RfqLPCostingProcessDtoUpdate>? RfqLPCostingProcesses { get; set; }
        public List<RfqLPCostingNREConsumableDtoUpdate>? RfqLPCostingNREConsumables { get; set; }
        public List<RfqLPCostingOtherChargesDtoUpdate>? RfqLPCostingOtherCharges { get; set; }

    }
}
