using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class KIT_GRIN_ProjectNumbers
    {
        [Key]
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal ProjectQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; } = 0;
        [Precision(18, 3)]
        public decimal RejectReturnQty { get; set; } = 0;
        [Precision(13, 3)]
        public decimal BinnedQty { get; set; } = 0;
        public bool IsKIT_IqcCompleted { get; set; } = false;
        public bool IsKIT_BinningCompleted { get; set; } = false;
        public int KIT_GRINPartsId { get; set; }
        public KIT_GRINParts? KIT_GRINParts { get; set; }
        public List<KIT_GRIN_KITComponents> KIT_GRIN_KITComponents { get; set; }      
    }
}
