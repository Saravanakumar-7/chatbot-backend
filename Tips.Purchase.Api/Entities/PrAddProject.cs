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
    public class PrAddProject
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? PrNumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; } 
        public int PrItemDetailId { get; set; }
        public PrItem? PrItemDetail { get; set; }
    }
}
