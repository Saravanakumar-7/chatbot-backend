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
    public class RfqCSDeliverySchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Precision(13,3)]
        public decimal Qty { get; set; }
        
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int RfqCustomerSupportItemsId { get; set; }
        public RfqCustomerSupportItems? RfqCustomerSupportItems { get; set; }
    }
}
