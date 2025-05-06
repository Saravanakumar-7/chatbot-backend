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
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
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
        public string? SerialNo { get; set; }
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
        public string? LotNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProjectNumber { get; set; }
        [Required]
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
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
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
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
        public string? SerialNo { get; set; }
        public string Unit { get; set; }
    }

    public class InventoryGrinDtoPost
    {
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        public string MftrPartNumber { get; set; }

        public string Description { get; set; }

        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? UOM { get; set; }

        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        public string? ReferenceID { get; set; }
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
        public string? MRNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public List<InventoryUpdateDtoForMRWarehouse> MRNWarehouseList { get; set; }
    }

    public class InventoryUpdateDtoForMRWarehouse
    {
        public string? LotNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        public bool IsMRIssueDone { get; set; }
    }

    public class MRNUpdateInventoryBalanceQty
    {
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? MRNNumber { get; set; }
        public List<MRNInventoryUpdateDto> MRNDetails { get; set; }
    }
    public class ItemNoWithPartTypeDto
    {
        public string ItemNumber { get; set; }

        public PartType PartType { get; set; }

    }
    public class MRNInventoryUpdateDto
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        public bool IsMRNIssueDone { get; set; }
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
    public class InventoryDtoForMaterialIssueLocation
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal DistributingQty { get; set; }
        public string ShopOrderNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
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
    public class ConsumptionInventoryByProjectNoDto
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }

    }
    public class ConsumptionChildItemInventoryDto
    {
        public string PartNumber { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal WipQuantity { get; set; }

    }
    public class ConsumptionChildItemForProjectListInventoryDto
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal WipQuantity { get; set; }

    }
    public class coverageInventoryByMultipleProjectDto
    {
        public List<string> itemNumberList { get; set; }
        public List<string> projectNo { get; set; }

    }
    public class InventoryBalanceQtyMaterialIssue
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal BalanceQty { get; set; }

    }
    public class InventoryQtyforDO
    {
        public string Warehouse { get; set; }
        public List<InventoryQtyforDOLocation>? inventoryQtyforDOLocations { get; set; }
    }
    public class InventoryQtyforDOLocation
    {
        public string LotNumber { get; set; }
        public string Location { get; set; }
        public decimal BalanceQty { get; set; }
    }
    public class GetInventoryQtyforDO
    {
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal BalanceQty { get; set; }
        public string LotNumber { get; set; }
    }
    public class GetInventoryItemNoAndDescriptionList
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
    }
    public class InventoryItemNoAndProjectNoDto
    {
        public string? FGItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class InventoryQtyForWeightedAvgCostDto
    {
        public string PartNumber { get; set; }
        public decimal BalanceQty { get; set; }
    }
    public class InventorySPReportDto
    {
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class GetInventorySPReportForAviDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class GetInventorySPReportForAvi
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Description { get; set; }
        public int? PartType { get; set; }
        public string? UOM { get; set; }
        public decimal? Opening_Stock { get; set; }
        public decimal? TotalInwords { get; set; }
        public decimal? Totaloutwords { get; set; }
        public decimal? Closing_stock { get; set; }
        public decimal? Price { get; set; }
        public decimal? ClosingStockValue { get; set; }
    }

    public class TrascationKPNWSPReportsDto
    {
        public string? KPN { get; set; }
    }

    public class InventoryWIPSPReportDto
    {
        public string? PartNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class InventoryWIPSPReport
    {
        public string? ProjectNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public int? PartType { get; set; }
        public string? UOM { get; set; }
        public decimal? Qty { get; set; }
        public string? UOC { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalValue { get; set; }
        public string? Warehouse { get; set; }
    }
    public class CrossMarginSPReportDto
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
    public class StockMovementHistorySPReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? ItemNumber { get; set; }
    }
    public class InventoryForStockSPReportDto
    {
        public string? PartNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
    }
    public class InventoryOqcBinningPostDto
    {
        [Required]
        public string PartNumber { get; set; }

        [Required]
        public string MftrPartNumber { get; set; }
        public string? LotNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProjectNumber { get; set; }
        [Required]
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
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
        public string? SerialNo { get; set; }

    }
    public class InventoryBySumOfFilteringDatesSPReport
    {
        public string? PartNumber { get; set; }
        public decimal? Opening_Stock { get; set; }
        public decimal? GrinBinningQty { get; set; }
        public decimal? OQCBinningQty { get; set; }
        public decimal? OpenGrinQty { get; set; }
        public decimal? MaterialReturnQty { get; set; }
        public decimal? ReturnInvoiceQty { get; set; }
        public decimal? ReturnOpenDeliveryQty { get; set; }
        public decimal? ReturnBtoDeliveryQty { get; set; }
        public decimal? MaterialIssueQty { get; set; }
        public decimal? MaterialRequestQty { get; set; }
        public decimal? OpenDeliveryOrderQty { get; set; }
        public decimal? BtoDeliveryOrderQty { get; set; }
        public decimal? TotalInwords { get; set; }
        public decimal? Totaloutwords { get; set; }
        public decimal? Closing_stock { get; set; }
    }
    public class InventoryBySumOfFilteringDatesSPReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? PartNumber { get; set; }
    }
    public class MRNIssueDetailsofMIandMR
    {
        public string ShopOrderNumber { get; set; }
        public List<MIDetailsfromMRN>? mIDetailsfromMRN { get; set; }
        public List<MRDetailsfromMRN>? mRDetailsfromMRN { get; set; }
    }
    public class MIDetailsfromMRN
    {
        public string PartNumber { get; set; }
        public decimal? QtyUsed { get; set; }
    }
    public class MRDetailsfromMRN
    {
        public string MRNumber { get; set; }
        public List<MIDetailsfromMRN>? items { get; set; }
    }
    public class InventoryDashboardSPReport
    {
        public string? Inventory { get; set; }
        public int? NumberOfCount { get; set; }
        public decimal? Value { get; set; }
    }
    public class InventoryDashboardSPReport_Details
    {
        public string Title { get; set; }
        public List<InventoryDashboardSPReport> Items { get; set; }
    }

    public class InventorySumSPReport
    {
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public int? PartType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOM { get; set; }
        public decimal? Opening_Stock { get; set; }
        public decimal? Value_Without_Tax_OpeningStock { get; set; }
        public decimal? GrinBinningQty { get; set; }
        public decimal? OpenGrinQty { get; set; }
        public decimal? MaterialReturnQty { get; set; }
        public decimal? ReturnInvoiceQty { get; set; }
        public decimal? ReturnOpenDeliveryQty { get; set; }
        public decimal? ReturnBtoDeliveryQty { get; set; }
        public decimal? OQCBinningQty { get; set; }
        public decimal? LocationTransferInQty { get; set; }
        public decimal? TotalInwords { get; set; }
        public decimal? Value_Without_Tax_inwards { get; set; }
        public decimal? MaterialIssueQty { get; set; }
        public decimal? MaterialRequestQty { get; set; }
        public decimal? OpenDeliveryOrderQty { get; set; }
        public decimal? BtoDeliveryOrderQty { get; set; }
        public decimal? LocationTransferOutQty { get; set; }
        public decimal? Totaloutwords { get; set; }
        public decimal? Value_Without_Tax_Outwards { get; set; }
        public decimal? Closing_stock { get; set; }
        public decimal? Price { get; set; }
        public decimal? ClosingStockValue { get; set; }
    }

}