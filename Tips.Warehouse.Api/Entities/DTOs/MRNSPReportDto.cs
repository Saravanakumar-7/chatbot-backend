using Tips.Warehouse.Api.Entities.Enums;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class MRNSPReportGetDto
    {
        public string? ProjectNumber { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? PartType { get; set; }
    }
 
}
