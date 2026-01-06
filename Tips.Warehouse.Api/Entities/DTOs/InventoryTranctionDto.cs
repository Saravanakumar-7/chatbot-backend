using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Entities.Enums;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InventoryTranctionDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal Issued_Quantity { get; set; }
        public string? UOM { get; set; }
        public DateTime Issued_DateTime { get; set; }
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public decimal BOM_Version_No { get; set; }
        public string? From_Location { get; set; }
        public string? TO_Location { get; set; }

        public bool ModifiedStatus { get; set; } = false;
        public string Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? shopOrderNo { get; set; }
        public string? BatchNo { get; set; }
        public string? SerialNo { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class InventoryTranctionDtoPost
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public PartType PartType { get; set; }
        public string ProjectNumber { get; set; }
        public InventoryType? TransactionType { get; set; }
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        public string? UOM { get; set; }
        public string? ShopOrderId { get; set; }
        public string ReferenceID { get; set; }
        public string ReferenceIDFrom { get; set; }

        public decimal? BOM_Version_No { get; set; }

        public string? From_Location { get; set; }
        public string TO_Location { get; set; }

        public bool? ModifiedStatus { get; set; } = false;

        public string? Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
        public string? BatchNo { get; set; }
        public string? SerialNo { get; set; }

    }

    public class InventoryTranctionDtoUpdate
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal Issued_Quantity { get; set; }

        public string? UOM { get; set; }
        public DateTime Issued_DateTime { get; set; }
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
       public decimal BOM_Version_No { get; set; }
        public string? From_Location { get; set; }
        public string? TO_Location { get; set; }
        public bool ModifiedStatus { get; set; } = false;

        public string Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? shopOrderNo { get; set; }
        public string? BatchNo { get; set; }
        public string? SerialNo { get; set; }


    }
    public class InventoryTranctionGrinDtoPost
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public PartType PartType { get; set; }
        public string ProjectNumber { get; set; }
        public InventoryType? TransactionType { get; set; }
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        public string? UOM { get; set; }
        public string? ShopOrderId { get; set; }
        public string ReferenceID { get; set; }
        public string ReferenceIDFrom { get; set; }

        public decimal? BOM_Version_No { get; set; }

        public string? From_Location { get; set; }
        public string TO_Location { get; set; }

        public bool? ModifiedStatus { get; set; } = false;

        public string? Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
    }
    public class InventoryTranctionDtoForShopOrderConfirmation
    {
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }

        public decimal NewConvertedToFgQty { get; set; }
        public string? DataFrom { get; set; }
        public string? MRNumber { get; set; }
    }
    public class InventoryTranctionBalanceQtyMaterialIssue
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Issued_Quantity { get; set; }

    }
    public class InventoryTranctionDtoForMaterialIssue
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal IssueQty { get; set; }

        public string ShopOrderNumber { get; set; }
        public decimal Bomversion { get; set; }


    }
    public class UpdateInventoryTranctionBalanceQty
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public List<InventoryTranctionUpdateDtoForMRWarehouse> MRNWarehouseList { get; set; }
    }

    public class InventoryTranctionUpdateDtoForMRWarehouse
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
    }

}
