using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities.DTOs
{
    public class OQCDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCPostDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCUpdateDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ShopOrderConfirmationItemNoListDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
    }
    public class ShopOrderConfirmationDetailsDto
    {
        public string? ShopOrderNumber { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public decimal? PendingQty { get; set; }

    }

    public class OQCIdNameList
    {
        public int Id { get; set; }

        public string? ShopOrderNumber { get; set; }
    }
    public class OQCSearchDto
    {
        //public List<string>? FGItemNumber { get; set; }
        //public List<string>? SAItemNumber { get; set; }
        public List<string>? ShopOrderNumber { get; set; }
        public List<decimal> ShopOrderQty { get; set; }
        public List<decimal> PendingQty { get; set; }
    }
    public class InventoryPostDto
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
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }

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
    public class InventoryTranctionDto
    {
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
        public string? Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
    }
    public class OQCStock
    {
        public string ItemNumber { get; set; }
        public string ShopOrderNumber { get; set; }
        public decimal TotalAcceptedQty { get; set; }
    }
    public class OQCSPReportDto
    {
        public string? ItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
    }
}
