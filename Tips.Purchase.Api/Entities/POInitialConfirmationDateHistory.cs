using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class POInitialConfirmationDateHistory
    {
        public int Id { get; set; }
        public int PoId { get; set; }
        public int POItemDetailId { get; set; }
        public string? PONumber { get; set; }
        public DateTime InitialConfirmationDate { get; set; }
        public decimal InitialQty { get; set; }
    }
}
