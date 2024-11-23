using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class IQCReturnToVendorItemsProject
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? InitialProjectQty { get; set; }
        [Precision(18, 3)]
        public decimal? ReturnQty { get; set; }

        public int iQCReturnToVendorItemsId { get; set; }
        public IQCReturnToVendorItems iQCReturnToVendorItems { get; set; }
    }
}
