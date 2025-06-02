using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_BinningItemsLocationPostDto
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
    public class KIT_BinningItemsLocationDto
    {
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
    }
}
