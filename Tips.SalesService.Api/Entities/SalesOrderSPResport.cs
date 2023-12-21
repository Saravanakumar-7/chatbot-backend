namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderSPResport
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? PartNumber { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? BalanceQty { get; set; }
    }


}