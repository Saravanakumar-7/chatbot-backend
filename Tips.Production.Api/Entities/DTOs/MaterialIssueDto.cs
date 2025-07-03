using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialIssueDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? BomRevisionNo { get; set; }
        public string? ProjectType { get; set; }
        public PartType ItemType { get; set; }
        [Precision(13, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public bool IsShortClosed { get; set; }
        public List<MaterialIssueItemDto> materialIssueItems { get; set; }
    }

    public class MaterialIssuePostDto
    {
        public string? ShopOrderNumber { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ProjectType { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? BomRevisionNo { get; set; }
        public PartType ItemType { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public bool IsShortClosed { get; set; }
        public List<MaterialIssueItemPostDto> MaterialIssueItems { get; set; }

    }

    public class MaterialIssueUpdateDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? ProjectType { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public bool IsShortClosed { get; set; }
        public List<MaterialIssueItemUpdateDto> MaterialIssueItems { get; set; }
    }
    public class IssueMaterialIssueUpdateDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? ProjectType { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public bool IsShortClosed { get; set; }
        public List<IssueMaterialIssueItemUpdateDto> MaterialIssueItems { get; set; }
    }
    public class MaterialIssueIdNameList
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
    }
    public class MaterialIssueSearchDto
    {
       // public List<string>? ItemType { get; set; }
        public List<string>? ShopOrderNumber { get; set; }
        //public List<string>? FGShopOrderNumber { get; set; }
        //public List<string>? SAShopOrderNumber { get; set; }
        public List<string>? ItemNumber { get; set; }
        //public List<string>? FGItemNumber { get; set; }
        //public List<string>? SAItemNumber { get; set; }
    }
    public class MaterialIssueReportDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectType { get; set; }

        public PartType ItemType { get; set; }
        [Precision(13, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }

        public List<MaterialIssueItemReportDto> materialIssueItems { get; set; }
    }
    public class MaterialIssuesDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? BomRevisionNo { get; set; }
        public string? ProjectType { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType ItemType { get; set; }
        [Precision(13, 3)]
        public decimal? ShopOrderQty { get; set; }
        public bool IsShortClosed { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; }
        public bool IsShopOrderconfirmed { get; set; } = false;
        public List<MaterialIssueItemsDto> materialIssueItems { get; set; }
    }
    public class MaterialIssueReportWithParamDto
    {
        public string? ShopOrderNumber { get; set; }
        public string? FGitemnumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
    }
    public class MaterialIssueReportWithParamDtoForTrans
    {
        public string? WorkorderNo { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
    }

    public class MaterialIssueSPReportForAvision
    {
        public string? WorkorderNo { get; set; }
        public DateTime? WorkOrderDate { get; set; }
        public decimal? WOreleaseqty { get; set; }
        public string? ProjectType { get; set; }
        public int? ItemType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemNumberchildlevel { get; set; }
        public string? latestMPN { get; set; }
        public decimal? BOMversion { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? SalesorderQnty { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? AvailableQnty { get; set; }
        public decimal? IssuedQty { get; set; }
        public decimal? BalanceIssueQnty { get; set; }
        public string? lotnumber { get; set; }
        public string? Remarks { get; set; }

    }

    public class MaterialIssueInvDetails
    {
        public Datass data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
    public class Datass
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
}
