using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class OpenDeliveryOrderPartsDto
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string ODONumber { get; set; }
        public PartTypes ItemType { get; set; }

        [Required]
        public string ItemDescription { get; set; }
        public string Warehouse { get; set; }

        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        public decimal StockAvailable { get; set; }
        public string Location { get; set; }
        public decimal LocationStock { get; set; }

        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }
        public string Remarks { get; set; }
        public string? SerialNo { get; set; }
        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OpenDeliveryOrderPartsDtoPost
    {

        [Required(ErrorMessage = "ItemNo is required")]
        public string ItemNumber { get; set; }

        [Required(ErrorMessage = "ItemType is required")]
        public PartTypes ItemType { get; set; }

        [Required]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "UOC is required")]
        public string UOC { get; set; }

        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }

        [Required(ErrorMessage = "StockAvailable is required")]
        public decimal StockAvailable { get; set; }
        public string Warehouse { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "LocationStock is required")]
        public decimal LocationStock { get; set; }

        [Required(ErrorMessage = "DispatchQty is required")]
        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }

        [Required(ErrorMessage = "Remarks is required")]
        public string Remarks { get; set; }
        public string? SerialNo { get; set; }

        public bool IsActive { get; set; } = true;

        
        
    }
    public class OpenDeliveryOrderPartsDtoUpdate
    {


        [Required(ErrorMessage = "ItemNo is required")]
        public string ItemNumber { get; set; }

        [Required(ErrorMessage = "ItemType is required")]
        public PartTypes ItemType { get; set; }

        [Required]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        public string Warehouse { get; set; }

        [Required(ErrorMessage = "UOC is required")]
        public string UOC { get; set; }

        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }

        [Required(ErrorMessage = "StockAvailable is required")]
        public decimal StockAvailable { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "LocationStock is required")]
        public decimal LocationStock { get; set; }

        [Required(ErrorMessage = "DispatchQty is required")]
        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }

        [Required(ErrorMessage = "Remarks is required")]
        public string? SerialNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        
    }
}
