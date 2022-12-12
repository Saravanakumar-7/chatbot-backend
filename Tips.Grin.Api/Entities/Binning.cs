using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class Binning
    {
        [Key]
        public int Id { get; set; }
        public string? SelectGrinNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? PONumber { get; set; }
        public string? VendorName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? CreatedBy { get; set; }
        public string unit { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; } = false;

        public List<BinningItems>? binningItems { get; set; }
        //public List<BinningLocation>? binningLocations { get; set; }



    }
}