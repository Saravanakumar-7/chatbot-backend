using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class ForeCastCustomerSupportItem
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public int? Quantity { get; set; }
        public string? Description { get; set; }
        public bool ReleaseStatus { get; set; } = false;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int ForeCastCustomerSupportId { get; set; }
        public ForeCastCustomerSupport? foreCastCustomerSupport { get; set; }
        public List<ForeCastCSDeliverySchedule>? foreCastCSDeliverySchedule { get; set; }
    }
}
