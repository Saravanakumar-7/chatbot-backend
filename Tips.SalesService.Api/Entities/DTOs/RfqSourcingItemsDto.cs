using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqSourcingItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }

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

        [Precision(13, 3)]
        public decimal? QtyReq { get; set; }

        [Precision(13, 3)]
        public decimal? Count { get; set; }

        public List<RfqSourcingVendorPostDto>? RfqSourcingVendorPostDtos { get; set; }

    }
    public class RfqSourcingItemsUpdateDto
    {
       
        [StringLength(500, ErrorMessage = "ItemNumber can't be longer than 100 characters")]
        public string? ItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }

        [Precision(13, 3)]
        public decimal? QtyReq { get; set; }

        [Precision(13, 3)]
        public decimal? Count { get; set; }
        public List<RfqSourcingVendorUpdateDto>? RfqSourcingVendorUpdateDtos { get; set; }

    }
}
