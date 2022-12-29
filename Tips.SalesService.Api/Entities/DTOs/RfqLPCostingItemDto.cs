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
        public List<RfqLPCostingProcessDto>? rfqLPCostingProcesses { get; set; }
        public List<RfqLPCostingNREConsumableDto>? rfqLPCostingNREConsumables { get; set; }
        public List<RfqLPCostingOtherChargesDto>? rfqLPCostingOtherCharges { get; set; }

    }
    public class RfqLPCostingItemDtoPost
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
        public List<RfqLPCostingProcessDtoPost>? rfqLPCostingProcesses { get; set; }
        public List<RfqLPCostingNREConsumableDtoPost>? rfqLPCostingNREConsumables { get; set; }
        public List<RfqLPCostingOtherChargesDtoPost>? rfqLPCostingOtherCharges { get; set; }

    }
    public class RfqLPCostingItemDtoUpdate
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
        public List<RfqLPCostingProcessDtoUpdate>? rfqLPCostingProcesses { get; set; }
        public List<RfqLPCostingNREConsumableDtoUpdate>? rfqLPCostingNREConsumables { get; set; }
        public List<RfqLPCostingOtherChargesDtoUpdate>? rfqLPCostingOtherCharges { get; set; }

    }
}
