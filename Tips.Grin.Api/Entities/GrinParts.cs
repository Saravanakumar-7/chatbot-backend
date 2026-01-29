using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.Purchase.Api.Entities;

namespace Tips.Grin.Api.Entities
{
    public class GrinParts
    {
        [Key]
        public int Id { get; set; }

        [Required]

        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }

        public PartType ItemType { get; set; }

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
        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }
        [Required]
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }

       // [Required]
        [Precision(13, 3)]
        public decimal? POOrderQty { get; set; }

        //[Required]
        [Precision(13, 3)]
        public decimal? POBalancedQty { get; set; }

        //[Required]
        [Precision(18, 3)]
        public decimal? POUnitPrice { get; set; }

        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        [Precision(13, 3)]
        public decimal? AverageCost { get; set; }
        [Precision(13, 3)]
        public decimal RejectReturnQty { get; set; }

        [Required]
        public string UOM { get; set; }
        public string? UOC { get; set; }
        public string? Remarks { get; set; }
        [DefaultValue(0)]
        public GrinStatus Status { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public bool IsCOCUploaded { get; set; } = false;

        public string? CoCUpload { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? SerialNo { get; set; }
        public decimal? InvoiceQty { get; set; }
        public string? InvoiceUOM { get; set; }
        public int GrinsId { get; set; }
        public Grins? Grins { get; set; }

        public List<ProjectNumbers>? ProjectNumbers { get; set; }
    }
}
