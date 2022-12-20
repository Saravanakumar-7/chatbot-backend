using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Entities
{
    public class PoItem
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        [Precision(13,3)]
        public decimal UnitPrice { get; set; }
        [Precision(13,2)]
        public decimal Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        [Precision(13, 3)]
        public decimal SGST { get; set; }
        [Precision(13, 3)]
        public decimal CGST { get; set; }
        [Precision(13, 3)]
        public decimal IGST { get; set; }
        [Precision(13, 3)]
        public decimal UTGST { get; set; }
        [Precision(13, 3)]
        public decimal Total { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        public List<PoAddProject>? PoAddprojects { get; set; }
        public List<PoAddDeliverySchedule>? PoAddDeliverySchedules { get; set; }
        
    }
}
