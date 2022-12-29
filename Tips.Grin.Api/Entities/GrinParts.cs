using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities
{
    public class GrinParts
    {
        public int Id { get; set; }

        public string ItemNumber { get; set; } 
            
        public string ItemDescription { get; set; }

        public string MftrItemNumber { get; set; }

        public string ProjectNumber { get; set; }

        public string ManufactureBatchNumber { get; set; }

        [Precision(18,3)]
        public decimal UnitPrice { get; set; }

        [Precision(13,3)]
        public decimal POOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal POBalancedQty { get; set; }

        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string? COCUpload { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }
        
        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int GrinsId { get; set; }
        public Grins? Grins { get; set; }

    }
}
