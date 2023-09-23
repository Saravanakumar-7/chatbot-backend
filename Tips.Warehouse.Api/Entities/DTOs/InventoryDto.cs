using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }
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
        public PartType PartType { get; set; }
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
        public PartType PartType { get; set; }
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
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string Unit { get; set; }
    }

    public class InventoryGrinDtoPost
    {
        [Required]
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

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
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        [Required]
        public string? ReferenceID { get; set; }
        [Required]
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

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
        public PartType PartType { get; set; }


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
    public class InventoryDetailsBalQty
    {
        public List<string> PartNumber { get; set; }
        public List<string> Warehouse { get; set; }
        public List<string> Location { get; set; }
        public List<string> ProjectNumber { get; set; }

    }
    public class InventoryItemNo
    {
        public string PartNumber { get; set; }
    }

    //public class UpdateInventoryBalanceQty
    //{
    //    public string? PartNumber { get; set; }
    //    public List<MRStockDetailsUpdateDto> MRStockDetails { get; set; }
    //}
    //public class MRStockDetailsUpdateDto
    //{
    //    public string? Warehouse { get; set; }
    //    public string? Location { get; set; }

    //    [Precision(13, 3)]
    //    public decimal LocationStock { get; set; }
    //}
    public class UpdateInventoryBalanceQty
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public List<InventoryUpdateDtoForMRWarehouse> MRNWarehouseList { get; set; }
    }

    public class InventoryUpdateDtoForMRWarehouse
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
    }

    public class MRNUpdateInventoryBalanceQty
    {
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }

        public List<MRNInventoryUpdateDto> MRNDetails { get; set; }
    }

    public class MRNInventoryUpdateDto
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
    }
    public class InventoryDetailsLocationStock
    {
        public string PartNumber { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string ProjectNumber { get; set; }

        public decimal LocationStock { get; set; }

    }
    public class OGInventoryDtoPost
    {
        [Required]
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

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
        public bool IsStockAvailable { get; set; }
        public string Unit { get; set; }

        [Required]
        public string? Warehouse { get; set; }
        [Required]
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        [Required]
        public string? ReferenceID { get; set; }
        [Required]
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

    }
    

    public class InventoryDtoForMaterialIssue
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal IssueQty { get; set; }
        
        public string ShopOrderNumber { get; set; }
        public decimal Bomversion { get; set; }


    }

    public class InventoryDtoForShopOrderConfirmation
    {
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }

        public decimal NewConvertedToFgQty { get; set; }
        public string? DataFrom { get; set; }
        public string? MRNumber { get; set; } 
    }

    public class ConsumptionInventoryDto
    {
        public string PartNumber { get; set; }
       public decimal Balance_Quantity { get; set; }

    }
    public class ConsumptionChildItemInventoryDto
    {
        public string PartNumber { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal WipQuantity { get; set; }

    }

    public class InventoryBalanceQtyMaterialIssue
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal BalanceQty { get; set; }

    }
    public class GetInventoryItemNoAndDescriptionList
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
    }

}