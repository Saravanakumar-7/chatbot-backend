using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinParts
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public bool Returnable { get; set; }
        public PartType ItemType { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal Qty { get; set; }
        public string? SerialNo { get; set; }
        public string? ReferenceSONumber { get; set; }
        public string? Remarks { get; set; }
        public int OpenGrinId { get; set; }
        public OpenGrin? OpenGrin { get; set; }
        public List<OpenGrinDetails> OpenGrinDetails { get; set; }

    }
}
