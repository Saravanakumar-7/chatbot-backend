using Entities.Enums;

namespace Tips.Warehouse.Api.Entities
{
    public class MaterialIssueTracker
    {
        public int Id { get; set; }
        public string ShopOrderNumber { get; set; }
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public string ProjectNumber { get; set; }
        public decimal IssuedQty { get; set; }
        public decimal ConvertedToFgQty { get; set; }
        public string? UOM { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string Unit { get; set; }
        public PartType PartType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        
    }
}
