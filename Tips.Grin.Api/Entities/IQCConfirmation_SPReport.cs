using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class IQCConfirmation_SPReport
    {
        public string? GrinNumber { get; set; }

        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? IqcDate { get; set; }
        public string? ItemNumber { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
    }
}
