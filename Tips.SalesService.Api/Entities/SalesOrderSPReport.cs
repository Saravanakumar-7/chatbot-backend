namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderSPReport
    {
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }   
        public string? SalesOrderNumber { get; set; }
        public DateTime? sodate { get; set; }
        public string? KPN { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? BalanceQty { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? OrderType { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? LeadId { get; set; }
        public string? SalesPerson { get; set; }
        public string? DiscountType { get; set; }
        public string? Discount { get; set; }
        public string? PriceList { get; set; }

    }


}