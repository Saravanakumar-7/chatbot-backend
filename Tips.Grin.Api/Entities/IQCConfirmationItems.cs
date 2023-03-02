using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Entities
{
    public class IQCConfirmationItems
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        [Precision(13,3)]
        public decimal ReceivedQty { get; set; }

        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public int IQCConfirmationId { get; set; }
        public IQCConfirmation? IQCConfirmation { get; set; }

    }
}
