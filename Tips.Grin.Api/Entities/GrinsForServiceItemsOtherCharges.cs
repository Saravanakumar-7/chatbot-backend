using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class GrinsForServiceItemsOtherCharges
    {
        [Key]
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
        public int GrinsForServiceItemsId { get; set; }
        public GrinsForServiceItems? GrinsForServiceItems { get; set; }
    }
}
