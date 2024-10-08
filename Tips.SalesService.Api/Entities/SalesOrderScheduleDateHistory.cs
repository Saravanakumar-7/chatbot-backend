using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderScheduleDateHistory
    {
        public int Id { get; set; }
        public int SOScheduleDateId { get; set; }
        public DateTime? Date { get; set; }

        [Precision(13, 3)]
        public decimal? Quantity { get; set; }
        public int SalesOrderItemLevelHistoryId { get; set; }
        public SalesOrderItemLevelHistory? SalesOrderItemLevelHistory { get; set; }
    }
}
