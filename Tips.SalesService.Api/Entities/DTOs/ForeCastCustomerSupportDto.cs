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
    public class ForeCastCustomerSupportDto
    {
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
        public List<ForeCastCustomerSupportItemDto>? ForeCastCustomerSupportItems { get; set; }

        public List<ForeCastCustomerSupportNotesDto>? ForeCastCustomerSupportNotes { get; set; }
    }
    public class ForeCastCustomerSupportPostDto
    {
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public string ForecastNumber { get; set; }
        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }
        public string? CustomerForecastNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }

        public List<ForeCastCustomerSupportItemPostDto>? ForeCastCustomerSupportItems { get; set; }
        public List<ForeCastCustomerSupportNotesPostDto>? ForeCastCustomerSupportNotes { get; set; }
    }
    public class ForeCastCustomerSupportUpdateDto
    {
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerForecastNumber { get; set; }

        public string ForecastNumber { get; set; }
        public DateTime? RequestReceivedDate { get; set; }
        public DateTime? QuoteExpectedDate { get; set; }

        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }

        public string Unit { get; set; }
      
        public List<ForeCastCustomerSupportItemUpdateDto>? ForeCastCustomerSupportItems { get; set; }
        public List<ForeCastCustomerSupportNotesUpdateDto>? ForeCastCustomerSupportNotes { get; set; }
    }
    public class ForeCAstCustomerSupportUpdateReleaseDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }
        public string ForeCastNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerForecastxxNumber can't be longer than 500 characters")]
        public string? CustomerForecastNumber { get; set; }
        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<FpreCastCustomerSupportItemUpdateReleaseDto>? ForeCastCustomerSupportItems { get; set; }
        public List<ForeCastCustomerSupportNotesUpdateDto>? ForecastCustomerSupportNotes { get; set; }

    }
}
