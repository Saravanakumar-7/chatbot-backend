using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinForBinningLocations
    {
        [Key]
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
        public int OpenGrinForBinningItemsId { get; set; }
        public OpenGrinForBinningItems? OpenGrinForBinningItems { get; set; }
    }
}
