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
    public class Rfq
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public int RevisionNumber { get; set; }
        public string RfqNumber { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public string? ReasonForModification { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public bool? IsSourcing { get; set; } = false;
        public bool? IsCsComplete { get; set; } = false;
        public CsStatus CsComplete { get; set; } 
        public CsRelease IsCsRelease { get; set; }
        public EnggStatus EnggComplete { get; set; }
        public CsRelease IsEnggRelease { get; set; }
        public bool? IsEnggComplete { get; set; } = false;
        public bool? IsLpCosting { get; set; } = false;
        public bool? IsLpCostingRelease { get; set; } = false;
        public bool? isSourcingAvailable { get; set; } = false;
        public bool IsModified { get; set; } = false;

        [DefaultValue(0)]
        public CsRelease ReleaseStatus { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

   
}
