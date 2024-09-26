using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForBinningLocationsDto
    {
        public int Id { get; set; }
        public string? ReferenceSONumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OpenGrinForBinningLocationsPostDto
    {
        public string? ReferenceSONumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
    public class OpenGrinForBinningLocationsUpdateDto
    {
        public string? ReferenceSONumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
    public class OpenGrinForBinningLocationsSaveDto
    {
        public string? ReferenceSONumber { get; set; }

        public string? Warehouse { get; set; }

        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }

    }
}
