using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class RfqLPCostingItem
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
        public string? MarkUpForProcessSteps { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqLPCostingId { get; set; }
        public RfqLPCosting? RfqLPCosting { get; set; }
        public List<RfqLPCostingProcess>? RfqLPCostingProcesses { get; set; }
        public List<RfqLPCostingNREConsumable>? RfqLPCostingNREConsumables { get; set; }
        public List<RfqLPCostingOtherCharges>? RfqLPCostingOtherCharges { get; set; }

    }
}
