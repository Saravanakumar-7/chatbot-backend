using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PoAddDeliverySchedule
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? PONumber { get; set; }
        public DateTime PODeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PODeliveryQty { get; set; } 
        public int POItemDetailId { get; set; }
        public PoItem? POItemDetail { get; set; }
    }
}
