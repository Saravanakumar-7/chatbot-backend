using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqCustomerSupportDto
    {
        public int Id { get; set; }

        public string? ItemNumber { get; set; }

        public int? Quantity { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? Notes { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int? ScheduleQuantity { get; set; }

        public string? SelectCustomGroup { get; set; }

        public DateTime? CustomFieldSchedule { get; set; }

        public string? SelectCustomGroups { get; set; }

        public string? LabelName { get; set; }

        public string? Type { get; set; }
        //engg field
        public string? ItemMaster { get; set; }
        //public string? EnggCategory { get; set; }

        public string? MaxLengthUnit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqCustomerSupportPostDto
    {
        public string? ItemNumber { get; set; }

        public int? Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? Notes { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int? ScheduleQuantity { get; set; }

        public string? SelectCustomGroup { get; set; }

        public DateTime? CustomFieldSchedule { get; set; }

        public string? SelectCustomGroups { get; set; }

        public string? LabelName { get; set; }

        public string? Type { get; set; }
        //engg fields
        public string? ItemMaster { get; set; }
        //public string? EnggCategory { get; set; }

        public string? MaxLengthUnit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqCustomerSupportUpdateDto
    {
        public int Id { get; set; }

        public string? ItemNumber { get; set; }

        public int? Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? Notes { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int? ScheduleQuantity { get; set; }

        public string? SelectCustomGroup { get; set; }

        public DateTime? CustomFieldSchedule { get; set; }

        public string? SelectCustomGroups { get; set; }

        public string? LabelName { get; set; }

        public string? Type { get; set; }

        //engg fields
        public string? ItemMaster { get; set; }
        //public string? EnggCategory { get; set; }

        public string? MaxLengthUnit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }


}
