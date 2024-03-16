namespace Tips.Production.Api.Entities.DTOs
{
    public class PickListDTO
    {
        public string? ShopOrderNumber { get;set; }
        public string? ItemNumber { get;set; }
        public decimal? ShopOrderQty { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public decimal? MaterialIssueId { get; set; }
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? IssuedQty { get; set; }
        public string? InventoryData { get; set; }
    }
    public class PickListGetDTO
    {
        public string? ShopOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public decimal? MaterialIssueId { get; set; }
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? IssuedQty { get; set; }
        public List<Invdata>? InventoryDatas { get; set; }
    }
    public class Invdata
    {
        public string? Location { get; set; }
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? Balance_Quantity { get; set; }
        public string? Warehouse { get; set; }
        public string? LotNumber { get; set; }
    }

}
