using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class InventoryTranction
    {
        [Key]
        public int Id { get; set; }
         
        public string? PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public PartType? PartType { get; set; }
        public string? ProjectNumber { get; set; }
         
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }
         
        public string? UOM { get; set; }

         public DateTime? Issued_DateTime { get; set; }

         public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

        public decimal? BOM_Version_No { get; set; }
         
        public string? From_Location { get; set; } 
        public string? TO_Location { get; set; }

        public bool? ModifiedStatus { get; set; } = false;
         
        public string? Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; } 
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; } 
        public string? LastModifiedBy { get; set; }        
        public DateTime? LastModifiedOn { get; set; }
    }
}
