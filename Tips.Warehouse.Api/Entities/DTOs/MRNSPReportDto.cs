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
    public class MRNSPReportGetDtoForTrans
    {
        public string? ProjectNumber { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartType { get; set; }
    }
    public class MRNSPReportForTrans
    {
        public string? MRNDate { get; set; }
        public string? ProjectNumber { get; set; }
        public int? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public int? PartType { get; set; }
        public string? Uom { get; set; }
        public string? MRNNumber { get; set; }
        public decimal? BalanceIssuedQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public string? CreatedBy { get; set; }
        public int? MrnStatus { get; set; }
    }


}
