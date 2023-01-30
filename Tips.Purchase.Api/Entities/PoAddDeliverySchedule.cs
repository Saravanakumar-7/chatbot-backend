using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PoAddDeliverySchedule
    {
        [Key]
        public int Id { get; set; }
        public DateTime PODeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PODeliveryQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int POItemDetailId { get; set; }
        public PoItem? POItemDetail { get; set; }
    }
}
