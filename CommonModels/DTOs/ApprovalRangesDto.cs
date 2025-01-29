using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ApprovalRangesDto
    {
        public int? Id { get; set; }
        public string ProcurementName { get; set; }
        public string? Description { get; set; }
        public List<RangesDto> Ranges { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ApprovalRangesPostDto
    {       
        public string ProcurementName { get; set; }
        public string? Description { get; set; }
        public List<RangesPostDto> Ranges { get; set; }
        public string? Remarks { get; set; }
    }
}
