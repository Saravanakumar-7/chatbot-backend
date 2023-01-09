using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EnggAlternates
    {
        public int Id { get; set; }

        public string ChildItemNumber { get; set; }
        public string ChildItemAlternateNumber { get; set; }

        public string? UOM { get; set; }

        [Precision(13,3)]
        public decimal QuantityPer { get; set; }

        public string? Description { get; set; }

        public string? Remarks { get; set; }

        public string? Version { get; set; }

        public string? ProbabilityOfUsage { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
         

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int EnggChildItemId { get; set; }
        public EnggChildItem? EnggChildItem { get; set; }


    }
}
