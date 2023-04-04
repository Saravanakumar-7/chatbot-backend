using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MRNWarehouseDetailsDto
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? ReturnQty { get; set; }
    }
    public class MRNWarehouseDetailsPostDto
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal? ReturnQty { get; set; }
    }
    public class MRNWarehouseDetailsUpdateDto
    {
        public int Id { get; set; }

        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal? ReturnQty { get; set; }
    }
}
