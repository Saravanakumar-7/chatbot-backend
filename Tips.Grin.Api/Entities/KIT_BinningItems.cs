using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class KIT_BinningItems
    {
        [Key]
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public int KIT_GrinPartId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int KIT_BinningId { get; set; }
        public KIT_Binning KIT_Binning { get; set; }
        public List<KIT_BinningItemsLocation> KIT_BinningItemsLocation { get; set; }
    }
}
