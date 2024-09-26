using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EnggBom
    {
        [Key]
        public int BOMId { get; set; }

        public string ItemNumber { get; set; }

        public string? ItemDescription { get; set; }

        public PartType ItemType { get; set; }

        [Precision(5, 2)]
        public decimal RevisionNumber { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        [DefaultValue(false)]
        public bool IsEnggBomRelease { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggChildItem>? EnggChildItems { get; set; }
        public List<NREConsumable>? NREConsumable { get; set; }

    }
}
