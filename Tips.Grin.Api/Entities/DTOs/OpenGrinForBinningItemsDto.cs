using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForBinningItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? AcceptedQty { get; set; }
        public decimal? RejectedQty { get; set;}
        public List<OpenGrinForBinningLocations>? OpenGrinForBinningLocations { get; set; }
    }
    public class OpenGrinForBinningItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }

        public List<OpenGrinForBinningLocations>? OpenGrinForBinningLocations { get; set; }
    }
    public class OpenGrinForBinningItemsUpdateDto
    {
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }

        public List<OpenGrinForBinningLocations>? OpenGrinForBinningLocations { get; set; }
    }
    public class OpenGrinForBinningItemsSaveDto
    {
        public string? ItemNumber { get; set; }
        public int OpenGrinForGrinItemId { get; set; }

        // Change property name to match the main class
        public List<OpenGrinForBinningLocationsSaveDto>? OpenGrinForBinningLocationsSaveDto { get; set; }
    }
}
