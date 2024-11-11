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
    public class RfqLPCostingProcessDto
    {
        public int Id { get; set; }
        public string? ProcessSteps { get; set; }
        public string? MachineHrs { get; set; }
        [Precision(13, 3)]
        public decimal? MachineHrsCost { get; set; }
        public string? LabourHrs { get; set; }
        [Precision(13, 3)]
        public decimal? LabourHrsCost { get; set; }
        //public string? MarkUpForProcessSteps { get; set; }       
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqLPCostingProcessDtoPost
    {
        public string? ProcessSteps { get; set; }
        public string? MachineHrs { get; set; }
       
        public decimal? MachineHrsCost { get; set; }
        public string? LabourHrs { get; set; }
       
        public decimal? LabourHrsCost { get; set; }
        //public string? MarkUpForProcessSteps { get; set; }
    }
    public class RfqLPCostingProcessDtoUpdate
    {
        public int Id { get; set; }
        public string? ProcessSteps { get; set; }
        public string? MachineHrs { get; set; }
        [Precision(13, 3)]
        public decimal? MachineHrsCost { get; set; }
        public string? LabourHrs { get; set; }
        [Precision(13, 3)]
        public decimal? LabourHrsCost { get; set; }
        //public string? MarkUpForProcessSteps { get; set; }        
    }

    public class RfqLpCostingItemProcessStepsDto
    {
        public string? ProcessSteps { get; set; }
        public string? MachineHrs { get; set; }
        [Precision(13, 3)]
        public decimal? MachineHrsCost { get; set; }
        public string? LabourHrs { get; set; }
        [Precision(13, 3)]
        public decimal? LabourHrsCost { get; set; }
    }


}
