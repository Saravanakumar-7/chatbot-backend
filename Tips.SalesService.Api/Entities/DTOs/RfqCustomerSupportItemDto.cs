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
        public string? ItemNumber { get; set; }         
        public int? Quantity { get; set; }
        public string? Description { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqCSDeliveryScheduleDto>? rfqCSDeliverySchedules { get; set; }

    }
    public class RfqCustomerSupportItemPostDto
    {
        public string? ItemNumber { get; set; }

        public int? Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqCSDeliverySchedulePostDto>? rfqCSDeliverySchedules { get; set; }

    }
    public class RfqCustomerSupportItemUpdateDto
    {
        public int Id { get; set; }

        public string? ItemNumber { get; set; } 

        public int? Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqCSDeliveryScheduleUpdateDto>? rfqCSDeliverySchedules { get; set; }

    }


}
