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
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? RevisionNumber { get; set; }
        public string? ForecastNumber { get; set; }
        public string? CustomerForecastReference { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ForeCastCustomerSupportItem>? foreCastCustomerSupportItems { get; set; }
        public List<ForeCastCustomerSupportNotes>? foreCastCustomerSupportNotes { get; set; }
    }
}
