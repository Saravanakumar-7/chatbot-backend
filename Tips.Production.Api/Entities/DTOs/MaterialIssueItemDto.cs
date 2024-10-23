using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialIssueItemDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }

        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        [Precision(13, 3)]
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public decimal? MRNQty { get; set; }
        public int MaterialIssueId { get; set; }
        public List<MaterialIssueLocationDto>? MaterialIssueLocationDto { get; set; }

    }

    public class MaterialIssueItemPostDto
    {
        public string? PartNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        public List<MaterialIssueLocationPostDto>? MaterialIssueLocationDto { get; set; }

    }

    public class MaterialIssueItemUpdateDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; } 
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }

        [Precision(13, 3)]
        public decimal NewIssueQty { get; set; }

        [Precision(13, 3)]
        public string? Unit { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public int MaterialIssueId { get; set; }
        public List<MaterialIssueLocationUpdateDto>? MaterialIssueLocationDto { get; set; }

    }
    public class IssueMaterialIssueItemUpdateDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }

        [Precision(13, 3)]
        public decimal NewIssueQty { get; set; }

        [Precision(13, 3)]
        public string? Unit { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public int MaterialIssueId { get; set; }
        public List<IssueMaterialIssueLocationUpdateDto> MaterialIssueLocationDto { get; set; }

    }
    public class MaterialIssueItemReportDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? PartNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }

        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        [Precision(13, 3)]
        public string? Unit { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public int MaterialIssueId { get; set; }

    }


    public class InventoryDtoForMaterialIssue
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal IssueQty { get; set; }
        public string? DataFrom { get; set; }

        public string ShopOrderNumber { get; set; }

        public decimal Bomversion { get; set; }

    }
    public class InventoryDtoForMaterialIssueLocation
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string? DataFrom { get; set; }
        public string ShopOrderNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
        public decimal Bomversion { get; set; }

    }
    public class InventoryTranctionForMaterialIssue
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
    public class InventoryBalanceQtyMaterialIssue
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal BalanceQty { get; set; }

    }
    public class MaterialIssueItemsDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }

        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }
        [Precision(13, 3)]
        public decimal AvailableQty { get; set; }
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        [Precision(13, 3)]
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public int MaterialIssueId { get; set; }
        public decimal? MRNQty { get; set; }
    }
}
