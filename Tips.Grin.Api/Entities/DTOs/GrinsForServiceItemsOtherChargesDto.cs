using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class GrinsForServiceItemsOtherChargesDto
    {
        [Key]
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
    public class GrinsForServiceItemsOtherChargesPostDto
    {
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
    public class GrinsForServiceItemsOtherChargesUpdateDto
    {
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
}
