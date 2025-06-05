using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class MRNWarehouseDetails
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        [Precision(13, 3)]
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public bool IsMRNIssueDone { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string? IssuedBy { get; set; }

        public int? MaterialReturnNoteItemId { get; set; }
        public MaterialReturnNoteItem? MaterialReturnNoteItem { get; set; }
    }
}
