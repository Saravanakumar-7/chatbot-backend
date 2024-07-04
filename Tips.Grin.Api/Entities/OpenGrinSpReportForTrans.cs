namespace Tips.Grin.Api.Entities
{
    public class OpenGrinSpReportForTrans
    {
        public string? OpenGrinNumber { get; set; }
        public DateTime? OpenGrinDate { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiptRefNo { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal? Qty { get; set; }
        public string? LotNumber { get; set; }
        public int? ItemType { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? SerialNo { get; set; }
        public string? Remarks { get; set; }
    }
}
