using System.ComponentModel.DataAnnotations;
using Entities.Enums;

namespace Tips.Warehouse.Api.Entities
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public string? UOM { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string Unit { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? shopOrderNo { get; set; }
        public string? SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
