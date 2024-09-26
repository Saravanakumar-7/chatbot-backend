using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class NREConsumable
    {
        [Key]
        public int Id { get; set; }
        public string? NREItemNumber { get; set; }
        [Precision(13,3)]
        public decimal? NREQuantity { get; set; }
        [Precision(18,3)]
        public decimal? NRECost { get; set; }
        public string? Description { get; set; } 
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int EnggBomId { get; set; }
        public EnggBom? EnggBom { get; set; }
    }
}
