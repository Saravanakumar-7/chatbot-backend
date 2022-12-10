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
        public int Id { get; set; }
        public DateTime PrDeliveryDate { get; set; }
        public decimal PrDeliveryQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PrItemsId { get; set; }
        public PrItems? PrItems { get; set; }
    }
}
