using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinForGrinItems
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? LotNumber { get; set; }
        public bool Returnable { get; set; }
        public PartType ItemType { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal Qty { get; set; }
        public string? SerialNo { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public bool IsOpenGrinForIqcCompleted { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public string? Remarks { get; set; }
        public int OpenGrinForGrinId { get; set; }
        public OpenGrinForGrin? OpenGrinForGrin { get; set; }
        public List<OpenGrinForGrinProjectNumber>? OGNProjectNumber { get; set; }
    }
}
