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
    public class ForeCastCustomerSupportItemDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string ForecastNumber { get; set; }


        [Precision(13,3)]
        public decimal? Qty { get; set; }
        public string? Description { get; set; }
        public bool ReleaseStatus { get; set; }       
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ForeCastCSDeliveryScheduleDto>? ForeCastCSDeliverySchedule { get; set; }
    }
    public class ForeCastCustomerSupportItemPostDto
    {
        [Required]
        public string ForecastNumber { get; set; }

        public string? ItemNumber { get; set; }

        [Precision(13,3)]
        public decimal? Qty { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
      

        public List<ForeCastCSDeliverySchedulePostDto>? ForeCastCSDeliverySchedule { get; set; }

    }
    public class ForeCastCustomerSupportItemUpdateDto
    {
        public string ForecastNumber { get; set; }

        public string? ItemNumber { get; set; }
        public int? Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }      
        public List<ForeCastCSDeliveryScheduleUpdateDto>? ForeCastCSDeliverySchedule { get; set; }

    }
    public class FpreCastCustomerSupportItemUpdateReleaseDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }
        public string ForeCastNumber { get; set; }
        public bool ReleaseStatus { get; set; } = true;

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerForeCastNumber { get; set; }
        public string? RevisionNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ForeCastCSDeliveryScheduleUpdateDto>? ForeCastCSDeliverySchedules { get; set; }

    }
}
