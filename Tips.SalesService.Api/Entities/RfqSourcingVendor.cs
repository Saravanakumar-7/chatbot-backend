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
    public class RfqSourcingVendor
    {
        
        public int Id { get; set; }
        public string? Vendor { get; set; }

        [Precision(13, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitPricePer { get; set; }
        public string? Currency { get; set; }
        public string? MOQ { get; set; }
        public string? LeadTime { get; set; }
        public string? Freight { get; set; }      
        public string? Duties { get; set; }

        [Precision(13,3)]
        public decimal? QuoteQuantity { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteValidity { get; set; }       
        public bool IsActive { get; set; } = true;        
        public int RfqSourcingItemsId { get; set; }
        public RfqSourcingItems? RfqSourcingItems { get; set; }
    }
}
