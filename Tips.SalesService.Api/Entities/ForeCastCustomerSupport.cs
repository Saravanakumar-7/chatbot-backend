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
    public class ForeCastCustomerSupport
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public string ForecastNumber { get; set; }
        public string? CustomerForecastNumber { get; set; }

        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }

        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ForeCastCustomerSupportItem>? ForeCastCustomerSupportItems { get; set; }
        public List<ForeCastCustomerSupportNotes>? ForeCastCustomerSupportNotes { get; set; }
    }
}
