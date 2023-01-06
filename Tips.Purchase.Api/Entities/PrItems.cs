using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class PrItem
    {
        public int Id { get; set; } 
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 2)]
        public decimal? Qty { get; set; }
        public string? SpecialInstruction { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int PurchaseRequistionId { get; set; }
        public PurchaseRequisition? PurchaseRequistion { get; set; }

        public List<PrAddProject>? PrAddprojects { get; set; }
        public List<PrAddDeliverySchedule>? PrAddDeliverySchedules { get; set; }
    }
}
