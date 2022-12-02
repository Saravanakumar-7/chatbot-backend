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
        
        public int Id { get; set; }
        public int? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public int? QuantityReq { get; set;}       
        public int? Count { get; set;}
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqSourcingId { get; set; }
        public RfqSourcing? RfqSourcing { get; set; }

        public List<RfqSourcingVendor>? rfqSourcingVendors { get; set; }
    }
}
