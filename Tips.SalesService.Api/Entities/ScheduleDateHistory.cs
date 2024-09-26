using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class ScheduleDateHistory
    {
        [Key]
        public int? Id { get; set; }
        public DateTime? Date { get; set; }

        [Precision(13, 3)]
        public decimal? Quantity { get; set; }

        public int SalesOrderHistoriesId { get; set; }
        public SalesOrderHistory? SalesOrderHistories { get; set; }
    }
}
