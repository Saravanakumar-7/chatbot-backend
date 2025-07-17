using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class KIT_IQCItems
    {
        [Key]
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public int KIT_GrinPartId { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectReturnQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal BinnedQty { get; set; } = 0;
        public string? Remarks { get; set; }
        public bool IsKIT_BinningCompleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int KIT_IQCId { get; set; }
        public KIT_IQC? KIT_IQC { get; set; }
    }
}
