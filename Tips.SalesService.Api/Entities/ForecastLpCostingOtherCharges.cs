using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class ForecastLpCostingOtherCharges
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        [Precision(13, 8)]
        public decimal? Value { get; set; }        
        public DateTime? LastModifiedOn { get; set; }
        public int ForeCastLPCostingItemId { get; set; }
        public ForecastLpCostingItem? ForecastLpCostingItem { get; set; }
    }
}
