using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class IQCForServiceItems_Items
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int GrinsForServiceItemsPartsId { get; set; }
        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }

        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
        public bool IsIqcForServiceItemsCompleted { get; set; }
        public int IQCForServiceItemsId { get; set; }
        public IQCForServiceItems? IQCForServiceItems { get; set; }
    }
}
