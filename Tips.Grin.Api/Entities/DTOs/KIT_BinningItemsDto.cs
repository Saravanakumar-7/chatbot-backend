using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_BinningItemsPostDto
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public int KIT_GrinPartId { get; set; }        
        public List<KIT_BinningItemsLocationPostDto> KIT_BinningItemsLocation { get; set; }
    }
    public class KIT_BinningItemsDto
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public int KIT_GrinPartId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int KIT_BinningId { get; set; }
        public List<KIT_BinningItemsLocationDto> KIT_BinningItemsLocation { get; set; }
    }
}
