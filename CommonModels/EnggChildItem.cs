using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EnggChildItem
    {
        public int Id { get; set; }

        public string ItemNumber { get; set; }

        public string? UOM { get; set; }

        [Precision(13,3)]
        public decimal Quantity { get; set; }

        public string? Description { get; set; }

        public string? Remarks { get; set; }

        public string? Version { get; set; }

        public string? ScrapAllowance { get; set; }

        public string? ScrapAllowanceType { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int EnggBomId { get; set; }
        public EnggBom? EnggBom { get; set; }

        public List<EnggAlternates>? EnggAlternates { get; set; }

        public NREConsumable? NREConsumable { get; set; }



    }
}
