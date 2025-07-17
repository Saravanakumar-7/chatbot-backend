using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class KIT_IQC
    {
        [Key]
        public int Id { get; set; }
        public string? KIT_IQCNumber { get; set; }
        public string KIT_GrinNumber { get; set; }
        public int KIT_GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string? VendorNumber { get; set; }
        public bool IsKIT_BinningCompleted { get; set; } = false;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<KIT_IQCItems> kIT_IQCItems { get; set; }
    }
}
