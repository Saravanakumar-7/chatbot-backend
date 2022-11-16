using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class GrinParts
    {
        public int Id { get; set; }

        public string ItemNODescription { get; set; }

        public string MftrItemNumber { get; set; }

        public string ProjectNumber { get; set; }

        public string ManufactureBatchNumber { get; set; }

        public decimal UnitPrice { get; set; }

        public int POOrderQuantity { get; set; }

        public int POBalancedQuantity { get; set; }

        public decimal POUnitPrice { get; set; }

        public int Quantity { get; set; }

        public string UOM { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public string COCUpload { get; set; }
        public decimal? SGST { get; set; }

        public decimal? IGST { get; set; }

        public decimal? CGST { get; set; }

        public decimal? UTGST { get; set; }


        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int GrinId { get; set; }

        public Grin? Grin { get; set; }
    }
}
