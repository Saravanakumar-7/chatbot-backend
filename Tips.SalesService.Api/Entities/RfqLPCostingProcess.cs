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
    public class RfqLPCostingProcess
    {
        
        public int Id { get; set; }
        public string? ProcessSteps { get; set; }
        public string? MachineHrs { get; set; }
        [Precision(13, 3)]
        public decimal? MachineHrsCost { get; set; }
        public string? LabourHrs { get; set; }
        [Precision(13, 3)]
        public decimal? LabourHrsCost { get; set; }
        public string? MarkUpForProcessSteps { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqLPCostingItemId { get; set; }
        public RfqLPCostingItem? rfqLPCostingItem { get; set; }
    }
}
