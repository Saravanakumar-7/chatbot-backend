using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForGrinItemsDto
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
        public string? ReferenceSONumber { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public bool IsOpenGrinForIqcCompleted { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public string? Remarks { get; set; }
        public List<OpenGrinForGrinProjectNumberDto>? OGNProjectNumberDto { get; set; }
    }
    public class OpenGrinForGrinItemsPostDto
    {
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
        public List<OpenGrinForGrinProjectNumberPostDto>? OGNProjectNumberDto { get; set; }
    }
    public class OpenGrinForGrinItemsUpdateDto
    {
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
        public List<OpenGrinForGrinProjectNumberUpdateDto>? OGNProjectNumberDto { get; set; }
    }
    public class OpenGrinForIQCConfirmationItemsSaveDto
    {
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }

    }
}
