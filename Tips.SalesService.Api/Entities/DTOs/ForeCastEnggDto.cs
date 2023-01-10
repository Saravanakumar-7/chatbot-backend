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
    public class ForeCastEnggDto
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
        public List<ForeCastEnggItemsDto>? ForeCastEnggItems { get; set; }
        public List<ForeCastEnggRiskIdentificationDto>? ForecastEnggRiskIdentifications { get; set; }

    }
    public class ForeCastEnggPostDto
    {
       
        public string? CustomerName { get; set; }
        public string? ForecastNumber { get; set; }
        public string? CustomerForecastReference { get; }
        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }
        public string? RevisionNumber { get; set; }
        public List<ForeCastEnggItemsPostDto>? ForeCastEnggItems { get; set; }
        public List<ForeCastEnggRiskIdentificationPostDto>? ForeCastEnggRiskIdentifications { get; set; }

    }
    public class ForeCastEnggUpdateDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? ForecastNumber { get; set; }
        public string? CustomerForecastReference { get; }
        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }
        public string? RevisionNumber { get; set; }
        public string Unit { get; set; }
                public List<ForeCastEnggItemsUpdateDto>? ForeCastEnggItems { get; set; }
        public List<ForeCastEnggRiskIdentificationUpdateDto>? RoreCastEnggRiskIdentifications { get; set; }

    }
}
