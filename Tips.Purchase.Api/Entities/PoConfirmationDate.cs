using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PoConfirmationDate
    {
        [Key]
        public int Id { get; set; }
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public int POItemDetailId { get; set; }
        public PoItem? POItemDetail { get; set; }
    }
}
