using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_BinningItemsLocationPostDto
    {
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
}
