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
    public class RfqSourcingVendorDto
    {
        public int Id { get; set; }
        public string? Vendor { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitPricePer { get; set; }
        public string? Currency { get; set; }
        public string? MOQ { get; set; }
        public string? LeadTime { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Duties { get; set; }

        [Precision(13, 3)]
        public decimal? QuoteQty { get; set; }
        [Precision(13, 3)]
        public decimal? LandingPrice { get; set; }
        [Precision(13, 3)]
        public decimal? MoqCost { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteValidity { get; set; }
        public bool Primary { get; set; }

    }
    public class RfqSourcingVendorPostDto
    {
        [StringLength(500, ErrorMessage = "Vendor can't be longer than 500 characters")]
        public string? Vendor { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        [Precision(13, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitPricePer { get; set; }

        [StringLength(500, ErrorMessage = "Currency can't be longer than 500 characters")]
        public string? Currency { get; set; }

        [StringLength(500, ErrorMessage = "MOQ can't be longer than 500 characters")]
        public string? MOQ { get; set; }

        [StringLength(500, ErrorMessage = "LeadTime can't be longer than 500 characters")]
        public string? LeadTime { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Duties { get; set; }

        [Precision(13, 3)]
        public decimal? QuoteQty { get; set; }
        [Precision(13, 3)]
        public decimal? LandingPrice { get; set; }
        [Precision(13, 3)]
        public decimal? MoqCost { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteValidity { get; set; }       
        public bool Primary { get; set; } = true;
         
    }
    public class RfqSourcingVendorUpdateDto
    {
       
        [StringLength(500, ErrorMessage = "Vendor can't be longer than 500 characters")]
        public string? Vendor { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        [Precision(13, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitPricePer { get; set; }

        [StringLength(500, ErrorMessage = "Currency can't be longer than 500 characters")]
        public string? Currency { get; set; }

        [StringLength(500, ErrorMessage = "MOQ can't be longer than 500 characters")]
        public string? MOQ { get; set; }

        [StringLength(500, ErrorMessage = "LeadTime can't be longer than 500 characters")]
        public string? LeadTime { get; set; }

        [Precision(13, 3)]
        public decimal? Freight { get; set; }

        [Precision(13, 3)]
        public decimal? Duties { get; set; }

        [Precision(13, 3)]
        public decimal? QuoteQty { get; set; }
        [Precision(13, 3)]
        public decimal? LandingPrice { get; set; }
        [Precision(13, 3)]
        public decimal? MoqCost { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteValidity { get; set; }        
    }
    public class RfqSourcingConvertionrateDto
    {
        public Data Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public decimal ConvertionRate { get; set; }
        public string UOC { get; set; }
        public DateTime Date { get; set; }
        public bool ActiveStatus { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
