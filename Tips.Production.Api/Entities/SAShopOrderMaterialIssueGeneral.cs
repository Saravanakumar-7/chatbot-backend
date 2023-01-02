using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class SAShopOrderMaterialIssueGeneral
    {
        public int Id { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public string PartType { get; set; }
        public string UOM { get; set; }
        [Precision(13, 2)]
        public decimal Quantity { get; set; }
        [Precision(13, 2)]
        public decimal RequiredQuantity { get; set; }
        [Precision(13, 2)]
        public decimal AvailableQuantity { get; set; }
        [Precision(13, 2)]
        public decimal AlreadyIssuedQty { get; set; }
        [Precision(13, 2)]
        public decimal IssueQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int SAShopOrderMaterialIssueId { get; set; }
        public SAShopOrderMaterialIssue? SAShopOrderMaterialIssue { get; set; }
    }
}
