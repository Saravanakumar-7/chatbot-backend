using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public string RfqNumber { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public string? CustomerId { get; set; }
        public bool? isSourcingAvailable { get; set; }
        public bool? IsCsComplete { get; set; }
        public bool? IsEnggComplete { get; set; } 
        public bool? IsSourcing { get; set; }
        public bool? IsLpCosting { get; set; }

        [DefaultValue(0)]
        public CsRelease ReleaseStatus { get; set; }

        public string? ReasonForModification { get; set; }

        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string Unit { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqPostDto
    {
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        //[Required]
        //public string RfqNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }

    }
    public class RfqUpdateDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        //[Required]
        //public string RfqNumber { get; set; }
        public string? ReasonForModification { get; set; }

        public string? CustomerId { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string Unit { get; set; }
        public string? Remarks { get; set; }

    }
    public class RfqNumberListDto
    {
        public int Id { get; set; }       
        public string RfqNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
    }
}
