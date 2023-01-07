using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class FGShopOrderMaterialIssueGeneral
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string PartType { get; set; }
        public string UOM { get; set; }

        [Precision(13, 2)]
        public decimal Qty { get; set; }

        [Precision(13, 2)]
        public decimal RequiredQty { get; set; }

        [Precision(13, 2)]
        public decimal AvailableQty { get; set; }

        [Precision(13, 2)]
        public decimal AlreadyIssuedQty { get; set; }

        [Precision(13, 2)]
        public decimal IssueQty { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int FGShopOrderMaterialIssueId { get; set; }
        public FGShopOrderMaterialIssue? FGShopOrderMaterialIssue { get; set; }
    }
}
