using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class ForecastSourcingItems
    {
        
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }

        [Precision(13,3)]
        public decimal? QtyReq { get; set; }

        [Precision(13,3)]
        public decimal? Count { get; set; }
        
        public int ForeCastSourcingId { get; set; }
        public ForecastSourcing? ForecastSourcing { get; set; }

        public List<ForecastSourcingVendor>? ForecastSourcingVendors { get; set; }
    }
}
