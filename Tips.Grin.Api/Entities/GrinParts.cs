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
        [Key]
        public int Id { get; set; }

        [Required]

        public string? ItemNumber { get; set; }

        [Required]
        [Precision(18, 3)]
        public decimal? Qty { get; set; }

        [Required]
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }

        [Required]
        public string MftrItemNumber { get; set; }

        [Required]
        public string ManufactureBatchNumber { get; set; }

        [Required]
        [Precision(18,3)]
        public decimal UnitPrice { get; set; }

        [Required]
        [Precision(13,3)]
        public decimal POOrderQty { get; set; }

        [Required]
        [Precision(13, 3)]
        public decimal POBalancedQty { get; set; }

        [Required]
        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }

        [Required]
        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        //public List<DocumentUpload> COCUpload { get; set; }

        public string? COCUpload { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }
        
        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int GrinsId { get; set; }
        public Grins? Grins { get; set; }

        public List<ProjectNumbers>? ProjectNumbers { get; set; }
    }
}
