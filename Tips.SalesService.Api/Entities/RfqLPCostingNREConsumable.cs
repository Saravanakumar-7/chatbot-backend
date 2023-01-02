using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class RfqLPCostingNREConsumable
    {
        public int Id { get; set; }
        public int? NREQty { get; set; }
        [Precision(13, 3)]
        public decimal? NRECost { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqLPCostingItemId { get; set; }
        public RfqLPCostingItem? rfqLPCostingItem { get; set; }
    }
}
