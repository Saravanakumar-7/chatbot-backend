using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class OQCBinningLocation
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int OQCBinningId { get; set; }
    }
}
