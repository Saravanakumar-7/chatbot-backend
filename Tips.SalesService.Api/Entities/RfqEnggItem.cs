using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class RfqEnggItem
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerItemNumber { get; set; }
        public string Description { get; set; }
        public bool ReleaseStatus { get; set; } = false;

        [Precision(13,3)]
        public decimal Qty { get;set;}

        [Precision(13, 3)]
        public decimal? CostingBomVersionNo { get; set; }
        public string? ItemNumber { get; set; }       
        public int RfqEnggId { get; set; }
        public RfqEngg? RfqEngg { get; set; }
      

    }
}
