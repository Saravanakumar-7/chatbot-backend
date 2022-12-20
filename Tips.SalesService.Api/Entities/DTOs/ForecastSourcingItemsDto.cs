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
    public class ForecastSourcingItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public int? QuantityReq { get; set; }
        public int? Count { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ForecastSourcingVendorDto>? forecastSourcingVendors { get; set; }
    }
    public class ForecastSourcingItemsDtoPost
    {
        [StringLength(500, ErrorMessage = "ItemNumber can't be longer than 100 characters")]
        public string? ItemNumber { get; set; }
        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }
        public int? QuantityReq { get; set; }
        public int? Count { get; set; }
        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
        public List<ForecastSourcingVendorDtoPost>? forecastSourcingVendors { get; set; }

    }
    public class ForecastSourcingItemsDtoUpdate
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessage = "ItemNumber can't be longer than 100 characters")]
        public string? ItemNumber { get; set; }
        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }
        public int? QuantityReq { get; set; }
        public int? Count { get; set; }
        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
        public List<ForecastSourcingVendorDtoUpdate>? forecastSourcingVendors { get; set; }

    }
}
