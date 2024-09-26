using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PrSpecialInstruction
    {
        [Key]
        public int Id { get; set; }
        public string? SpecialInstruction { get; set; }
        public int PrItemDetailId { get; set; }
        public PrItem? PrItemDetail { get; set; }
    }
}
