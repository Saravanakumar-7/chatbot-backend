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
}
