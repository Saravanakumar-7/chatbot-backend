using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class SalerOrderConfirmationDateHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
}
