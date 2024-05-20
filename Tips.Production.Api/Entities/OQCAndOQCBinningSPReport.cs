namespace Tips.Production.Api.Entities
{
    public class OQCAndOQCBinningSPReport
    {
            public string? ItemNumber { get; set; } 
            public string? Description { get; set; } 
            public int? ItemType { get; set; } 
            public string? ShopOrderNumber { get; set; } 
            public string? ProjectNumber { get; set; } 
            public decimal? ShopOrderQty { get; set; } 
            public decimal? AcceptedQty { get; set; } 
            public decimal? pendingqnty { get; set; } 
            public decimal? RejectedQty { get; set; } 
            public decimal? binnigqnty { get; set; } 
            public string? Warehouse { get; set; } 
            public string? Location { get; set; } 
            public decimal? pendingbinning { get; set; }

    }
}
