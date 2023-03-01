using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Entities
{
    public class IQCConfirmationItems
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        public string? ReceivedQty { get; set; }
        public string? AcceptedQty { get; set; }
        public string? RejectedQty { get; set; }
        public int IQCConfirmationId { get; set; }
        public IQCConfirmation? IQCConfirmation { get; set; }

    }
}
