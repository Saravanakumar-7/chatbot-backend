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
    public class RfqEngg
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerName { get; set;}
        public string? CustomerAliasName { get; set; }
        public string? RFQNumber { get; set;}
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set;}

        [Precision(13,1)]
        public decimal? RevisionNumber { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqEnggItem>? RfqEnggItems { get; set; }
        public List<RfqEnggRiskIdentification>? RfqEnggRiskIdentifications { get; set; }
    }
}
