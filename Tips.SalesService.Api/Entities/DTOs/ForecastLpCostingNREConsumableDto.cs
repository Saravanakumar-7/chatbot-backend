using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ForecastLpCostingNREConsumableDto
    {
        public int Id { get; set; }
        [Precision(13, 3)]
        public decimal? NREQty { get; set; }
        [Precision(13, 3)]
        public decimal? NRECost { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class ForecastLPCostingNREConsumableDtoPost
    {
        [Precision(13, 3)]
        public decimal? NREQty { get; set; }
        [Precision(13, 3)]
        public decimal? NRECost { get; set; }

    }
    public class ForecastLPCostingNREConsumableDtoUpdate
    {
        [Precision(13, 3)]
        public decimal? NREQty { get; set; }
        [Precision(13, 3)]
        public decimal? NRECost { get; set; }

    }
}
