namespace Tips.Warehouse.Api.Entities
{
    public class StockMovementSPReport
    {
        public string ItemNumber { get; set; }
        public decimal ODOQTY { get; set; }
        public decimal RODOQTY { get; set; }
        public decimal DoQty { get; set; }
        public decimal RDoQty { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal oqcqty { get; set; }
        public decimal oqcbinningqty { get; set; }
        public decimal Stock_closing { get; set; }
    }
}
