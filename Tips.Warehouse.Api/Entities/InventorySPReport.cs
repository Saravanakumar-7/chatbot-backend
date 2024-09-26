using Entities.Enums;

namespace Tips.Warehouse.Api.Entities
{
    public class InventorySPReport
    {
        public string? PartNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public PartType PartType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOM { get; set; }
        public string? LotNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? MaterialGroup { get; set; }
    }
}
