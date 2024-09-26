using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinForBinningItems
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
        public int OpenGrinForGrinItemId { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public PartType ItemType { get; set; }
        public string? UOM { get; set; }
        public int OpenGrinForBinningId { get; set; }
        public OpenGrinForBinning? OpenGrinForBinning { get; set; }

        public List<OpenGrinForBinningLocations>? OpenGrinForBinningLocations { get; set; }
    }
}
