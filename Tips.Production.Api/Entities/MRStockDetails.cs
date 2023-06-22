using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class MRStockDetails
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        public int? MaterialRequestItemsId { get; set; }
        public MaterialRequestItems? MaterialRequestItems { get; set; }
    }
}
