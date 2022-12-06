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
        public int Id { get; set; }
        public string? CustomerItemNumber { get; set; }
        public string Description { get; set; }
        public int? Quantity { get;set;}
        public string? CostingBomVersionNo { get;}
        public string? ItemNumber { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqEnggId { get; set; }
        public RfqEngg? rfqEngg { get; set; }
      

    }
}
