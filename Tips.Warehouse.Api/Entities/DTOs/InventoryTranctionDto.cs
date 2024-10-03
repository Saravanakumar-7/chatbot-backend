using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Entities.Enums;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InventoryTranctionDto
    {
        public int Id { get; set; }

        [Display(Name = "Part Number")]
        public string? PartNumber { get; set; }
        public string? LotNumber { get; set; }

        [Display(Name = "Mftr Part Number")]
        public string? MftrPartNumber { get; set; }

        [Display(Name = "Description")]
        [StringLength(250, ErrorMessage = "String length exceeded")]
        public string? Description { get; set; }

        [Display(Name = "Project Number")]
        public string? ProjectNumber { get; set; }

        [Display(Name = "Issued Quantity")]
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        [Display(Name = "UOM")]
        public string? UOM { get; set; }

        [DataType(DataType.DateTime), Display(Name = "Issued DateTime")]
        public DateTime Issued_DateTime { get; set; }

        [StringLength(100), Display(Name = " Issued By")]
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

        [Display(Name = "BOM Version No")]
        public decimal BOM_Version_No { get; set; }

        [Display(Name = "From Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? From_Location { get; set; }

        [Display(Name = "To Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? TO_Location { get; set; }

        public bool ModifiedStatus { get; set; } = false;

        [Display(Name = "Unit Name")]
        public string Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? shopOrderNo { get; set; }

        [StringLength(100), Display(Name = "Created By")]
        public string? CreatedBy { get; set; }


        [DataType(DataType.DateTime), Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }


        [StringLength(100), Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }


        [DataType(DataType.DateTime), Display(Name = "Last Modified On")]
        public DateTime? LastModifiedOn { get; set; }
    }
    public class InventoryTranctionDtoPost
    {

        [Display(Name = "Part Number")]
        public string? PartNumber { get; set; }

        [Display(Name = "Mftr Part Number")]
        public string? MftrPartNumber { get; set; }

        [Display(Name = "Description")]        
        public string? Description { get; set; }

        [Display(Name = "Project Number")]
        public string? ProjectNumber { get; set; }

        [Display(Name = "Issued Quantity")]
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        [Display(Name = "UOM")]
        public string? UOM { get; set; }

        [DataType(DataType.DateTime), Display(Name = "Issued DateTime")]
        public DateTime Issued_DateTime { get; set; }

        [StringLength(100), Display(Name = " Issued By")]
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

        [Display(Name = "BOM Version No")]
        public decimal BOM_Version_No { get; set; }

        [Display(Name = "From Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? From_Location { get; set; }

        [Display(Name = "To Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? TO_Location { get; set; }

        public bool ModifiedStatus { get; set; } = false;   

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? shopOrderNo { get; set; }

    }

    public class InventoryTranctionDtoUpdate
    {
        public int Id { get; set; }

        [Display(Name = "Part Number")]
        public string? PartNumber { get; set; }

        [Display(Name = "Mftr Part Number")]
        public string? MftrPartNumber { get; set; }

        [Display(Name = "Description")]
        [StringLength(250, ErrorMessage = "String length exceeded")]
        public string? Description { get; set; }

        [Display(Name = "Project Number")]
        public string? ProjectNumber { get; set; }

        [Display(Name = "Issued Quantity")]
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        [Display(Name = "UOM")]
        public string? UOM { get; set; }

        [DataType(DataType.DateTime), Display(Name = "Issued DateTime")]
        public DateTime Issued_DateTime { get; set; }

        [StringLength(100), Display(Name = " Issued By")]
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

        [Display(Name = "BOM Version No")]
        public decimal BOM_Version_No { get; set; }

        [Display(Name = "From Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? From_Location { get; set; }

        [Display(Name = "To Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? TO_Location { get; set; }

        public bool ModifiedStatus { get; set; } = false;

        [Display(Name = "Unit Name")]
        public string Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? shopOrderNo { get; set; }


    }
    public class InventoryTranctionGrinDtoPost
    {

        [Display(Name = "Part Number")]
        public string? PartNumber { get; set; }
        public string? LotNumber { get; set; }

        [Display(Name = "Mftr Part Number")]
        public string? MftrPartNumber { get; set; }

        [Display(Name = "Description")]
        [StringLength(250, ErrorMessage = "String length exceeded")]
        public string? Description { get; set; }

        [Display(Name = "Project Number")]
        public string? ProjectNumber { get; set; }

        [Display(Name = "Issued Quantity")]
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        [Display(Name = "UOM")]
        public string? UOM { get; set; }

        [DataType(DataType.DateTime), Display(Name = "Issued DateTime")]
        public DateTime Issued_DateTime { get; set; }

        [StringLength(100), Display(Name = " Issued By")]
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

        [Display(Name = "BOM Version No")]
        public decimal BOM_Version_No { get; set; }

        [Display(Name = "From Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? From_Location { get; set; }

        [Display(Name = "To Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? TO_Location { get; set; }

        public bool ModifiedStatus { get; set; } = false;

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
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
