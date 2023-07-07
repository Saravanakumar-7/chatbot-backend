using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PoIncoTerm
    {
        [Key]
        public int Id { get; set; }
        public string? IncoTermName { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
    }

}
