using Entities.Enums;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrin_SPReport
    {
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiptRefNo { get; set; }
        public DateTime? Opengrindate { get; set; }
        public string? KPN { get; set; }
        public string? Description { get; set; }
        public PartType? ItemType { get; set; }
        public string? UOM { get; set; }
        public decimal Qty { get; set; }
        public string? SerialNo { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ProjectNumber { get; set; }
    }
}
