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
        public decimal ShopOrderQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? SerialNo { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? Quantity { get; set; }
    }
}
