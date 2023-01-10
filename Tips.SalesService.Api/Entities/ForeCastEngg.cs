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
    public class ForeCastEngg
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? ForecastNumber { get; set; }
        public string? CustomerForecastReference { get; }
        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }
        public string? RevisionNumber { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ForeCastEnggItems>? ForeCastEnggItems { get; set; }
        public List<ForeCastEnggRiskIdentification>? ForeCastEnggRiskIdentifications { get; set; }
    }
}
