using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PoSpecialInstruction
    {
        [Key]
        public int Id { get; set; }
        public string? SpecialInstruction { get; set; }
        public int POItemDetailId { get; set; }
        public PoItem? POItemDetail { get; set; }
    }
}
