using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class OtherCharges
    {

        [Key]
        public int Id { get; set; }
        public string? OtherCharge { get; set; }
        public int GrinsId { get; set; }
        public Grins? Grins { get; set; }
    }
}
