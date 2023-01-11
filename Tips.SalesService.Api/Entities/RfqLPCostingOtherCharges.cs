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
    public class RfqLPCostingOtherCharges
    {
        [Key]
        public int Id { get; set; }
        public string? NameOfLable { get; set; }       

        [Precision(13, 8)]
        public decimal? CostOfLable { get; set; }       
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqLPCostingItemId { get; set; }
        public RfqLPCostingItem? RfqLPCostingItem { get; set; }
    }
}
