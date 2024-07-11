using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class IQCForServiceItems
    {
        [Key]
        public int Id { get; set; }
        public string? IQCForServiceItemsNumber { get; set; }
        public string? GrinsForServiceItemsNumber { get; set; }
        public int GrinsForServiceItemsId { get; set; }
        public bool IsIqcForServiceItemsCompleted { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<IQCForServiceItems_Items>? IQCForServiceItems_Items { get; set; }
    }
}
