using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class FGShopOrderMaterialIssue
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrderDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGPartNumber { get; set; }
        [Precision(13, 2)]
        public decimal? ShopOrderQty { get; set; }
        public string? ShopOrderType { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<FGShopOrderMaterialIssueGeneral>? FGShopOrderMaterialIssueGeneralList { get; set; }
    }
}
