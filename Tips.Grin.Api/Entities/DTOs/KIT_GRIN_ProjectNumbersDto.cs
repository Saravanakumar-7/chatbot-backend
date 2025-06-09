using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_GRIN_ProjectNumbersPostDto
    {
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }
        public List<KIT_GRIN_KITComponentsPostDto> KIT_GRIN_KITComponents { get; set; }
    }
    public class KIT_GRIN_ProjectNumbersDto {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal ProjectQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        [Precision(18, 3)]
        public decimal RejectReturnQty { get; set; } 
        [Precision(13, 3)]
        public decimal BinnedQty { get; set; }
        public bool IsKIT_IqcCompleted { get; set; } = false;
        public bool IsKIT_BinningCompleted { get; set; } = false;
        public int KIT_GRINPartsId { get; set; }
        public List<KIT_GRIN_KITComponentsDto> KIT_GRIN_KITComponents { get; set; }
    }
    public class KIT_GRIN_ProjectNumbersUpdateDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }
        public List<KIT_GRIN_KITComponentsUpdateDto> KIT_GRIN_KITComponents { get; set; }
    }
}
