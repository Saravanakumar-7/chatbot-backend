using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_IQCItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public int KIT_GrinPartId { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class KIT_IQCItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public int KIT_GrinPartId { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectReturnQty { get; set; }
        public string? Remarks { get; set; }
        public bool IsKIT_BinningCompleted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
