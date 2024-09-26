using Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class MaterialIssueAgainstMRSPReport
    {
        public string? MRNumber { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ProjectType { get; set; }
        public PartType? ItemType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? indent_qnty { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? Qty { get; set; }
        public string? Uom { get; set; }
        public string? IssuedBy { get; set; }
        public string? CreatedBy { get; set; }
    }
}
