using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class KIT_GRIN_ProjectNumbers
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }
        [Precision(18, 3)]
        public decimal RejectReturnQty { get; set; }
        public List<KIT_GRIN_KITComponents> KIT_GRIN_KITComponents { get; set; }
        public int KIT_GRINPartsId { get; set; }
        public KIT_GRINParts? KIT_GRINParts { get; set; }
    }
}
