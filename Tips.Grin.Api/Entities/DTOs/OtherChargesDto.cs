using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OtherChargesDto
    {
        [Key]
        public int Id { get; set; }
        public string? OtherCharge { get; set; }
    }
    public class OtherChargesPostDto
    {
        public string? OtherCharge { get; set; }
    }
    public class OtherChargesUpdateDto
    {
        public string? OtherCharge { get; set; }
    }
}
