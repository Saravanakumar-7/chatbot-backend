using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SalesOrderScheduleDateHistoryDto
    {
        public int Id { get; set; }
        public int SOScheduleDateId { get; set; }
        public DateTime? Date { get; set; }

        [Precision(13, 3)]
        public decimal? Quantity { get; set; }
    }
}
