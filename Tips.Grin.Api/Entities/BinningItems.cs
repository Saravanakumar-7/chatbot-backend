using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities
{
    public class BinningItems
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ManufactureBatchNumber { get; set; }

        [Precision(18,3)]
        public decimal? UnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal? POOrderedQty { get; set; }

        [Precision(18, 3)]
        public decimal? POBalancedQty { get; set; }

        [Precision(18, 3)]
        public decimal? POUnitPrice { get; set; }
        
        public string? UOM { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        [Precision(18, 3)]
        public decimal? ReceivedQty { get; set; }

        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }

        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int BinningId { get; set; }
        public Binning? Binning { get; set; }

        public List<BinningLocation>? BinningLocations { get; set; }
    }
}