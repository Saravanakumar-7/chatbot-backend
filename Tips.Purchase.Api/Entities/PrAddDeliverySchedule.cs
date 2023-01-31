using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Purchase.Api.Entities
{
    public class PrAddDeliverySchedule
    {
        [Key]
        public int Id { get; set; }
        public DateTime PRDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PRDeliveryQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PrItemDetailId { get; set; }
        public PrItem? PrItemDetail { get; set; }
    }
}
