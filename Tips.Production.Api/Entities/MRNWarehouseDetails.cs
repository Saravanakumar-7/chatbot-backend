using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class MRNWarehouseDetails
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal? ReturnQty { get; set; }
    }
}
