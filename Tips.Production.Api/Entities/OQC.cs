using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class OQC
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
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
}
