using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class KIT_GRIN_OtherCharges
    {
        [Key]
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
        public int KIT_GRINId { get; set; }
        public KIT_GRIN? KIT_GRIN { get; set; }
    }
}
