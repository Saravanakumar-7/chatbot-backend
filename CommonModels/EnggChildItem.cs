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
    public class EnggChildItem
    {
        [Key]
        public int Id { get; set; }

        public string ItemNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? UOM { get; set; }

        [Precision(13,3)]
        public decimal Quantity { get; set; }

        public string? Description { get; set; }

        public PartType PartType { get; set; }

        public string? Remarks { get; set; }

        public string? Version { get; set; }

        public string? ScrapAllowance { get; set; }

        public string? ScrapAllowanceType { get; set; }
        public string? CustomFields { get; set; }
        public string? Designator {  get; set; }
        public string? FootPrint {  get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } 

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? Manufacturer_Mftr_PartNumber { get; set; }
        public string? Customer_Mftr_PartNumber { get; set; }
        public int EnggBomId { get; set; }
        public EnggBom? EnggBom { get; set; }

        public List<EnggAlternates>? EnggAlternates { get; set; }

       
    }
}
