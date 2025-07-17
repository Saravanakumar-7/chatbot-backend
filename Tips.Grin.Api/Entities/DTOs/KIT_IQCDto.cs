using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_IQCPostDto
    {
        public string KIT_GrinNumber { get; set; }
        public int KIT_GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string? VendorNumber { get; set; }
        public List<KIT_IQCItemsPostDto> kIT_IQCItems { get; set; }
    }
    public class KIT_IQCDto
    {
        public int Id { get; set; }
        public string? KIT_IQCNumber { get; set; }
        public string KIT_GrinNumber { get; set; }
        public int KIT_GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string? VendorNumber { get; set; }
        public bool IsKIT_BinningCompleted { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<KIT_IQCItemsDto> kIT_IQCItems { get; set; }
    }
}
