using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class BinningDto
    {
        public int Id { get; set; }
        public string? SelectGrinNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? PONumber { get; set; }
        public string? VendorName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<BinningItemsDto>? binningItems { get; set; }
        //public List<BinningLocationDto>? binningLocations { get; set; }
    }


    public class BinningPostDto
    {

        public string? SelectGrinNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? PONumber { get; set; }
        public string? VendorName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<BinningItemsPostDto>? binningItems { get; set; }
        //public List<BinningLocationPostDto>? binningLocations { get; set; }

    }

    public class BinningUpdateDto
    {

        public int Id { get; set; }
        public string? SelectGrinNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? PONumber { get; set; }
        public string? VendorName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BinningItemsUpdateDto>? binningItems { get; set; }
        //public List<BinningLocationUpdateDto>? binningLocations { get; set; }

    }
}