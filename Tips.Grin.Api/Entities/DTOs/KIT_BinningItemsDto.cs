namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_BinningItemsPostDto
    {
        public string ItemNumber { get; set; }
        public int KIT_GrinPartId { get; set; }        
        public List<KIT_BinningItemsLocationPostDto> KIT_BinningItemsLocation { get; set; }
    }
}
