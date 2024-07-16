using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForIQCItemsDto
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
        public string? Remarks { get; set; }
        public List<OpenGrinForGrinProjectNumberDto>? ReferenceSONumbers { get; set; }
    }
    public class OpenGrinForIQCItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class OpenGrinForIQCItemsUpdateDto
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class OpenGrinForIQCItemsSaveDto
    {
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
}
