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
        public decimal shoporderqnty { get; set; }
        public decimal OQCAcceptedQnty { get; set; }
        public decimal? BinningItemQnty { get; set; }
        public decimal pendingBinning { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
