using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCReturnToVendorItemsProjectPostDto
    {
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? InitialProjectQty { get; set; }
        [Precision(18, 3)]
        public decimal? ReturnQty { get; set; }
    }
}
