using Tips.Warehouse.Api.Entities.Enums;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class MRNSPReportGetDto
    {
        public string? ProjectNumber { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartType { get; set; }
    }
    public class MRNSPReportDto
    {
        public string? MRNdate { get; set; }
        public string? ProjectNumber { get; set; }
        public int? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public PartType? PartType { get; set; }
        public decimal? BalanceIssuedQty { get; set; }
        public decimal? ReturnQty { get; set; }

    }
}
