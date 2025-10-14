using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class OQC
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
        public ShopOrderConformationStatus IsOQCBinningDone { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public ShopOrderType ShopOrderType { get; set; }
    }
}
