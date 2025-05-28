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
}
