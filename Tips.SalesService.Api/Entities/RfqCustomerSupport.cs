using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class RfqCustomerSupport
    {
        public int Id { get; set; }

        public string? ItemNumber { get; set; }
        public int? Quantity { get; set; }

        public string? Description { get; set; }
     

        //public DateTime? ScheduleDate { get; set; }

        //public int? ScheduleQuantity { get; set; }

        //public string? SelectCustomGroup { get; set; }

        //public DateTime? CustomFieldSchedule { get; set; }

        //public string? SelectCustomGroups { get; set; }

        //public string? LabelName { get; set; }

        //public string? Type { get; set; }

        //public string? MaxLengthUnit { get; set; }

        ////enggfield
        //public string? ItemMaster { get; set; } 

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int RfqId { get; set; }
        public Rfq? Rfq { get; set; }

    }
}
