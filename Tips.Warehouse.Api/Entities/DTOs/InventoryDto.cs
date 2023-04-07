using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public string? UOM { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string Unit { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class InventoryDtoPost
    {
        [Required]
        public string PartNumber { get; set; }

        [Required]
        public string MftrPartNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProjectNumber { get; set; }
        [Required]
        public decimal Balance_Quantity { get; set; }
        [Required]
        public string? UOM { get; set; }
        
        [Required]
        public string? Warehouse { get; set; }
        [Required]
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        [Required]
        public string? ReferenceID { get; set; }
        [Required]
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
      
    }
    public class InventoryDtoUpdate
    {
        [Required]
        public string PartNumber { get; set; }

        [Required]
        public string MftrPartNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public string? UOM { get; set; }       
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string Unit { get; set; }
    }

    public class GetInventoryListByItemNo
    {
        public int InventoryId { get; set; }
        public string ItemNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
    }

    public class ListOfLocationTransferDto
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string? UOM { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string? PartType { get; set; }


    }
    public class InvoiceSearchDto
    {
        public List<string> CustomerName { get; set; }
        public List<string>? InvoiceNumber { get; set; }
        public List<string> CompanyName { get; set; }
    }
}
