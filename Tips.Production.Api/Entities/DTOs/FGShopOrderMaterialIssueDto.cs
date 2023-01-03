using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class FGShopOrderMaterialIssueDto
    {
        public int Id { get; set; }
        public string ShopOrderNo { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string ProjectNumber { get; set; }
        public string FGPartNo { get; set; }
        [Precision(13, 2)]
        public decimal ShopOrderQty { get; set; }
        public string ShopOrderType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<FGShopOrderMaterialIssueGeneralDto>? FGShopOrderMaterialIssueGeneralDtos { get; set; }
    }
    public class FGShopOrderMaterialIssueDtoPost
    {
        public int Id { get; set; }
        public string ShopOrderNo { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string ProjectNumber { get; set; }
        public string FGPartNo { get; set; }
        [Precision(13, 2)]
        public decimal ShopOrderQty { get; set; }
        public string ShopOrderType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<FGShopOrderMaterialIssueGeneralDtoPost>? FGShopOrderMaterialIssueGeneralPostDtos { get; set; }
    }
    public class FGShopOrderMaterialIssueDtoUpdate
    {
        public int Id { get; set; }
        public string ShopOrderNo { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string ProjectNumber { get; set; }
        public string FGPartNo { get; set; }
        [Precision(13, 2)]
        public decimal ShopOrderQty { get; set; }
        public string ShopOrderType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<FGShopOrderMaterialIssueGeneralDtoUpdate>? FGShopOrderMaterialIssueGeneralUpdateDtos { get; set; }
    }
}
