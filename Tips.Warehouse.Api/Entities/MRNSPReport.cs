namespace Tips.Warehouse.Api.Entities
{
    public class MRNSPReport
    {
        public string? KPN { get; set; }
        public string? Description { get; set; }
        public string? Uom { get; set; }
        public string? MRNNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public int? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public int? PartType { get; set; }
        public decimal? BalanceIssuedQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}