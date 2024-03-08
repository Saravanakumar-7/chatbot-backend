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
        public ProjectType ProjectType { get; set; }
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
        public ProjectType ProjectType { get; set; }
        public string? ItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public bool IsShortClosed { get; set; }
        public List<MaterialIssueItemPostDto> MaterialIssueItems { get; set; }

    }

    public class MaterialIssueUpdateDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public ProjectType ProjectType { get; set; }
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
        public ProjectType ProjectType { get; set; }

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
        public ProjectType ProjectType { get; set; }
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

        public List<MaterialIssueItemsDto> materialIssueItems { get; set; }
    }
    public class MaterialIssueReportWithParamDto
    {
        public string? ShopOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
    }
}
