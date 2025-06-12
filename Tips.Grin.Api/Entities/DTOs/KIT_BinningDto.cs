using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_BinningPostDto
    {
        public string KIT_GrinNumber { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public List<KIT_BinningItemsPostDto> KIT_BinningItems { get; set; }
    }
    public class KIT_BinningDto
    {
        public int Id { get; set; }
        public string KIT_GrinNumber { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<KIT_BinningItemsDto> KIT_BinningItems { get; set; }
    }
}
