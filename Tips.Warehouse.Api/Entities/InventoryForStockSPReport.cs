namespace Tips.Warehouse.Api.Entities
{
    public class InventoryForStockSPReport
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? qnty { get; set; }
    }
}
