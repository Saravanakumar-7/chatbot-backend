using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities
{
    public class PoAddProject
    {
        public int Id { get; set; }
        public string PoProjectNumber { get; set; }
        [Precision(13, 2)]
        public decimal PoProjectQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PoItemDetailId { get; set; }
        public PoItem? PoItemDetail { get; set; }

    }
}
