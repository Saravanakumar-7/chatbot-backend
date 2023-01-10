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
    public class ForeCast
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? RevisionNumber { get; set; }
        public string ForeCastNumber { get; set; }
        public string? CustomerForeCastRefrence { get; set; }
        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }
        public bool? IsSourcing { get; set; } = false;
        public bool? IsLpCosting { get; set; } = false;
        public bool? IsLpCostingRelease { get; set; } = false;
        public bool ReleaseStatus { get; set; } = false;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
