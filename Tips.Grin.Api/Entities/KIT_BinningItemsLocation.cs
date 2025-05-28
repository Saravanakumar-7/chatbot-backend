using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class KIT_BinningItemsLocation
    {
        [Key]
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int KIT_BinningItemsId { get; set; }
        public KIT_BinningItems KIT_BinningItems { get; set; }
    }
}
