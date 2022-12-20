using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class ForeCastCSDeliverySchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int ForeCastCustomerSupportItemId { get; set; }
        public ForeCastCustomerSupportItem? foreCastCustomerSupportItems { get; set; }
    }
}
