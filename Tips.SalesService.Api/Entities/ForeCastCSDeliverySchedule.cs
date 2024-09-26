using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class ForeCastCSDeliverySchedule
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int ForeCastCustomerSupportItemId { get; set; }
        public ForeCastCustomerSupportItem? ForeCastCustomerSupportItems { get; set; }
    }
}
