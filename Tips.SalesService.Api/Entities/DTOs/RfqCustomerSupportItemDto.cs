using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqCustomerSupportItemDto
    {
        public int Id { get; set; }
        public string RfqNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomFields { get; set; }

        [Precision(13,3)]
        public decimal Qty { get; set; }
        public string? Description { get; set; }
        public string? RoomName { get; set; }
        public bool ReleaseStatus { get; set; }       
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqCSDeliveryScheduleDto>? RfqCSDeliverySchedule { get; set; }

    }
    public class RfqCustomerSupportItemPostDto
    {
        [Required]
        public string RfqNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomFields { get; set; }

        [Precision(13,3)]
        public decimal Qty { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        public string? RoomName { get; set; }
        public List<RfqCSDeliverySchedulePostDto>? RfqCSDeliverySchedule { get; set; }

    }
    public class RfqCustomerSupportItemUpdateDto
    {
        [Required]
        public string RfqNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomFields { get; set; }

        [Precision(13,3)]
        public decimal Qty { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        public string? RoomName { get; set; }

        public List<RfqCSDeliveryScheduleUpdateDto>? RfqCSDeliverySchedule { get; set; }

    }
    public class RfqCustomerSupportItemUpdateReleaseDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }

        [Required]
        public string RFQNumber { get; set; }
        public bool ReleaseStatus { get; set; } = true;

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public string? RevisionNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }     

        public List<RfqCSDeliveryScheduleUpdateDto>? RfqCSDeliverySchedule { get; set; }

    }   
}
