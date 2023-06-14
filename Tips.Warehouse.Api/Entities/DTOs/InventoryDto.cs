using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;

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
    public class InventorySearchDto
    {
        public List<string> PartNumber { get; set; }
        public List<string> ProjectNumber { get; set; }
        public List<string> Warehouse { get; set; }
        public List<string>? Location { get; set; }
        public List<string>? GrinNo { get; set; }
    }

    public class InventoryUpdateDtoForMRN
    {
        public int? Id { get; set; }
        public string? ProjectNumber { get; set; }
        public string? MRNNumber { get; set; }
        public PartType ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<InventoryUpdateDtoForMRNItem>? MaterialReturnNoteItems { get; set; }
    }
    public class InventoryUpdateDtoForMRNItem
    {
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public PartType PartType { get; set; }
        public decimal ReturnQty { get; set; }
        public List<InventoryUpdateDtoForMRNWarehouse> MRNWarehouseList { get; set; }
    }
    public class InventoryUpdateDtoForMRNWarehouse
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
    }
    public class InventoryItemNoStock
    {
        public string PartNumber { get; set; }
        public decimal Balance_Quantity { get; set; }

    }
    public class InventoryBalQty
    {
        public List<string> PartNumber { get; set; }
        public List<string> Warehouse { get; set; }
        public List<string> Location { get; set; }

    }
    public class InventoryItemNo
    {
        public string PartNumber { get; set; }
    }
}