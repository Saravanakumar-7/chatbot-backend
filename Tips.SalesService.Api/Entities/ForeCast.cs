using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities.Enum;

namespace Tips.SalesService.Api.Entities
{
    public class ForeCast
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerName { get; set; }

        public string? CustomerAliasName { get; set; }
        public string? CustomerForecastNumber { get; set; }
        public int RevisionNumber { get; set; }
       

        public string? ReasonForModification { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public CsStatus CsComplete { get; set; }
        public bool? IsCsComplete { get; set; } = false;

        public string? CustomerId { get; set; }
        public CsRelease IsCsRelease { get; set; }

        public bool? IsLpCostingRelease { get; set; } = false;
        public bool? isSourcingAvailable { get; set; } = false;
        public string ForeCastNumber { get; set; }
        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }
        public bool? IsSourcing { get; set; } = false;

        [DefaultValue(0)]
        public CsRelease ReleaseStatus { get; set; }
        public bool IsModified { get; set; } = false;
        public string? Remarks { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
