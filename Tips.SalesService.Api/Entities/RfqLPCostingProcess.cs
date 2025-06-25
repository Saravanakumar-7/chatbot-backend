using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class RfqLPCostingProcess
    {
        [Key]
        public int Id { get; set; }
        public string? ProcessSteps { get; set; }
        public string? MachineHrs { get; set; }
        [Precision(13, 3)]
        public decimal? MachineHrsCost { get; set; }
        public string? LabourHrs { get; set; }
        [Precision(13, 3)]
        public decimal? LabourHrsCost { get; set; }    
        public int? NoOfDays {  get; set; }

        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        [NotMapped]
        public string? ItemNumber { get; set; }

        public int RfqLPCostingItemId { get; set; }
        public RfqLPCostingItem? RfqLPCostingItem { get; set; }
    }
}
