using Entities.Enums;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrin_SPReport
    {
        public string? KPN { get; set; }
        public string? Description { get; set; }
        public int ItemType { get; set; }
        public string? UOM { get; set; }
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiptRefNo { get; set; }
        public string? ReferenceSONumber { get; set; }
        public decimal? Qty { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public bool Returnable { get; set; }
        public string? Remarks { get; set; }
        public string? SerialNo { get; set; }
        public string? ReturnedBy { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
