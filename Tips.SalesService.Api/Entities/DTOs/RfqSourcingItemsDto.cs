using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqSourcingItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? Manufacturer_Mftr_PartNumber { get; set; }
        public string? Customer_Mftr_PartNumber { get; set; }
        [Precision(13, 3)]
        public decimal? QtyReq { get; set; }
        [Precision(13, 3)]
        public decimal? Count { get; set; }
        public List<RfqSourcingVendorDto>? RfqSourcingVendorDtos { get; set; }
    }
    public class RfqSourcingItemsPostDto
    {
        [StringLength(500, ErrorMessage = "ItemNumber can't be longer than 100 characters")]
        public string? ItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }
        public string? Manufacturer_Mftr_PartNumber { get; set; }
        public string? Customer_Mftr_PartNumber { get; set; }
        [Precision(13, 3)]
        public decimal? QtyReq { get; set; }

        [Precision(13, 3)]
        public decimal? Count { get; set; }

        public List<RfqSourcingVendorPostDto>? RfqSourcingVendorDtos { get; set; }

    }
    public class RfqSourcingItemsUpdateDto
    {
        public int? Id { get; set; }
        [StringLength(500, ErrorMessage = "ItemNumber can't be longer than 100 characters")]
        public string? ItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }
        public string? Manufacturer_Mftr_PartNumber { get; set; }
        public string? Customer_Mftr_PartNumber { get; set; }
        [Precision(13, 3)]
        public decimal? QtyReq { get; set; }

        [Precision(13, 3)]
        public decimal? Count { get; set; }
        public List<RfqSourcingVendorUpdateDto>? RfqSourcingVendorDtos { get; set; }

    }
}
