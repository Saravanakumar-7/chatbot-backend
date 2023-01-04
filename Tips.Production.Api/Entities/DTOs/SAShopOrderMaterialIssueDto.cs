using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class SAShopOrderMaterialIssueDto
    {
        public int Id { get; set; }
        public string SAShopOrderNumber { get; set; }
        public DateTime SAShopOrderDate { get; set; }
        public string ProjectNumber { get; set; }
        public string FGPartNumber { get; set; }
        [Precision(13, 2)]
        public decimal SAShopOrderQty { get; set; }
        public string ShopOrderType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SAShopOrderMaterialIssueGeneralDto>? SAShopOrderMaterialIssueGeneralDtos { get; set; }
    }
    public class SAShopOrderMaterialIssueDtoPost
    {
        public string SAShopOrderNumber { get; set; }
        public DateTime SAShopOrderDate { get; set; }
        public string ProjectNumber { get; set; }
        public string FGPartNumber { get; set; }
        [Precision(13, 2)]
        public decimal SAShopOrderQty { get; set; }
        public string ShopOrderType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SAShopOrderMaterialIssueGeneralDtoPost>? SAShopOrderMaterialIssueGeneralPostDtos { get; set; }
    }
    public class SAShopOrderMaterialIssueDtoUpdate
    {
        public int Id { get; set; }
        public string SAShopOrderNumber { get; set; }
        public DateTime SAShopOrderDate { get; set; }
        public string ProjectNumber { get; set; }
        public string FGPartNumber { get; set; }
        [Precision(13, 2)]
        public decimal SAShopOrderQty { get; set; }
        public string ShopOrderType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SAShopOrderMaterialIssueGeneralDtoUpdate>? SAShopOrderMaterialIssueGeneralUpdateDtos { get; set; }
    }
}
