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
        public List<RfqSourcingItemsDto>? RfqSourcingItemsDtos { get; set; }
        

    }
    public class RfqSourcingPostDto
    {
        public int RFQId { get; set; }
        [StringLength(500, ErrorMessage = "RFQNumber can't be longer than 100 characters")]
        public string? RFQNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }      
       
        public List<RfqSourcingItemsPostDto>? RfqSourcingItemsPostDtos { get; set; }
       



    }
    public class RfqSourcingUpdateDto
    {
        public int Id { get; set; }
        public int RFQId { get; set; }
        [StringLength(500, ErrorMessage = "RFQNumber can't be longer than 100 characters")]
        public string? RFQNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }

        [Required]
        public string Unit { get; set; }

        public List<RfqSourcingItemsUpdateDto>? RfqSourcingItemsUpdateDtos { get; set; }
    }
    public class RfqSourcingPPdetails
    {
        public string PPItemNumber { get; set; }
        public decimal? VLandindPrice { get; set; }
        public decimal? VMoqcost { get; set; }
    }

    public class SourcingSPReportDto
    {
        public string Vendor { get; set; }
    }
    public class SourcingSPReport
    {
        public string Vendor { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Cost { get; set; }
        public decimal Percent { get; set; }
        public decimal CumulativePercent { get; set; }
        public string Grade { get; set; }
    }

}
