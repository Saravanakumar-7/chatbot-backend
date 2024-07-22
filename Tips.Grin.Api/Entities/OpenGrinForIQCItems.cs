using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinForIQCItems
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }
        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }

        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public PartType ItemType { get; set; }
        public string? UOM { get; set;}
        public string? Remarks { get; set; }
        public bool IsOpenGrinForIqcCompleted { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public int OpenGrinForIQCId { get; set; }
        public OpenGrinForIQC? OpenGrinForIQC { get; set; }
    }
}
