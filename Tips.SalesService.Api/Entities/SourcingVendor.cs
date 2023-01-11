using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class SourcingVendor
    {
        [Key]
        public int Id { get; set; }

        public string? SourcingVendors { get; set; }

        public string? SourcingCurrency { get; set; }

        public string? Freight { get; set; }

        public DateTime? QuoteDate { get; set; }

        public string? UnitPrice { get; set; }

        public string? MOQ { get; set; }

        public string? Duties { get; set; }

        public DateTime? QuoteValidity { get; set; }
        public string? UnitPricePer { get; set; }
        public string? LeadTime { get; set; }
        public string? QuoteQuantity { get; set; }
        public string? UploadFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}
