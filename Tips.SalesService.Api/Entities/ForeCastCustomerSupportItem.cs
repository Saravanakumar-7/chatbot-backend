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
        [Key]
        public int Id { get; set; }

        public string ForecastNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomFields { get; set; }

        [Precision(13,3)]
        public decimal? Qty { get; set; }
        public string? Description { get; set; }
        public bool ReleaseStatus { get; set; } = false;      
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int ForeCastCustomerSupportId { get; set; }
        public ForeCastCustomerSupport? ForeCastCustomerSupport { get; set; }
        public List<ForeCastCSDeliverySchedule>? ForeCastCSDeliverySchedule { get; set; }
    }
}
