using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class SoConfirmationDate
    {
        [Key]
        public int Id { get; set; }
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public int SalesOrderItemsId { get; set; }
        public SalesOrderItems? SalesOrderItems { get; set; }
    }
}
