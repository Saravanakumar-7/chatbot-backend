using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class IQCReturnToVendor
    {
        [Key]
        public int Id { get; set; }
        public string? IQCNumber { get; set; }
        public string GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<IQCReturnToVendorItems> iQCReturnToVendorItems { get; set; }
    }
}
