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
    public class RfqSourcingDto
    {
        public int Id { get; set; }
        public string? RFQNumber { get; set; }
        public string? CustomerName { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqSourcingItemsDto>? rfqSourcingItems { get; set; }
        //public List<RfqSourcingVendorDto>? rfqSourcingVendors { get; set; }

    }
    public class RfqSourcingDtoPost
    {
        [StringLength(500, ErrorMessage = "RFQNumber can't be longer than 100 characters")]

        public string? RFQNumber { get; set; }
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]

        public string? CustomerName { get; set; }
       
        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
        public List<RfqSourcingItemsDtoPost>? rfqSourcingItems { get; set; }
        //public List<RfqSourcingVendorDtoPost>? rfqSourcingVendors { get; set; }



    }
    public class RfqSourcingDtoUpdate
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessage = "RFQNumber can't be longer than 100 characters")]

        public string? RFQNumber { get; set; }
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]

        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
        public List<RfqSourcingItemsDtoUpdate>? rfqSourcingItems { get; set; }
       // public List<RfqSourcingVendorDtoUpdate>? rfqSourcingVendors { get; set; }

    }
}
