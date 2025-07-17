using System;
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
    public class RfqSourcingItems
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? Manufacturer_Mftr_PartNumber { get; set; }
        public string? Customer_Mftr_PartNumber { get; set; }
        [Precision(13,3)]
        public decimal? QtyReq { get; set;}

        [Precision(13, 3)]
        public decimal? Count { get; set;}        
        public int RfqSourcingId { get; set; }
        public RfqSourcing? RfqSourcing { get; set; }

        public List<RfqSourcingVendor>? RfqSourcingVendors { get; set; }
    }
}
