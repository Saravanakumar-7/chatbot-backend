using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class OQCBinningSPReport
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal ShopOrderQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal PendingQty { get; set; }
        public decimal? RejectedQty { get; set; } 
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
