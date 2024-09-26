using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinDetails
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(18, 3)]
        public decimal Qty { get; set; }
        public int OpenGrinPartsId { get; set; }
        public OpenGrinParts? OpenGrinParts { get; set; }
    }
}
