namespace Tips.Production.Api.Entities.DTOs
{
    public class PickListProductionDTO
    {
        public string? ShopOrderNumber { get; set; }
    
    }
    public class PickListInventoryDTO
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public decimal? Balance_Quantity { get; set; }
    }
}
