namespace Tips.SalesService.Api.Entities
{
    public class SOSummarySPReport
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? socreateddate { get; set; }
        public string? RoomName { get; set; }
        public string? KPN { get; set; }
        public string? KPNDescription { get; set; }
        public decimal? MRP { get; set; }
        public decimal? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? DiscountType { get; set; }
        public string? Discount { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
