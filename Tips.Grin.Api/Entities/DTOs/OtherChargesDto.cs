using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OtherChargesDto
    {
        [Key]
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
    public class OtherChargesPostDto
    {
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
    public class OtherChargesUpdateDto
    {
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
}
