namespace Tips.SalesService.Api.Entities
{
    public class RfqSalesOrderRoomWiseSPReport
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? LeadId { get; set; }
        public string? OrderType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? MaterialGroup { get; set; }
        public string? SalesPerson { get; set; }
        public DateTime? sodate { get; set; }
        public string? RoomName { get; set; }
        public string? KPN { get; set; }
        public string? KPNDescription { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public string? PriceList { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? BasicAmount { get; set; }
        public string? DiscountType { get; set; }
        public string? Discount { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? itempricelist { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? BalanceQty { get; set; }
    }
}
