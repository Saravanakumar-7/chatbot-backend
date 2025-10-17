using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class DiscountRanges
    {
        [Key]
        public int Id { get; set; }
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public bool IsActive { get; set; }
        public List<DiscountUsers> DiscountUsers { get; set; }
    }
}
