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
    public class RfqCustomerSupportDto
    {   
        public int Id { get; set; }    
        public string? CustomerName { get; set; }
        public string RFQNumber { get; set; }
        public string? CustomerRfqNumber { get; set; }


        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<RfqCustomerSupportItemDto>? RfqCustomerSupportItems { get; set; }

        public List<RfqCustomerSupportNotesDto>? RfqCustomerSupportNotes { get; set; }


    }
    public class RfqCustomerSupportPostDto
    { 
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }        

        [Required]
        public string RFQNumber { get; set; }     


        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }

        [Precision(13,1)]
        public decimal? RevisionNumber { get; set; }

        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }       
        public List<RfqCustomerSupportItemPostDto>? RfqCustomerSupportItems { get; set; }
        public List<RfqCustomerSupportNotesPostDto>? RfqCustomerSupportNotes { get; set; }

    }
    public class RfqCustomerSupportUpdateDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }


        [Required]
        public string RFQNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }


        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
         
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }
        public string Unit { get; set; }
        public List<RfqCustomerSupportItemUpdateDto>? RfqCustomerSupportItems { get; set; }
        public List<RfqCustomerSupportNotesUpdateDto>? RfqCustomerSupportNotes { get; set; }

    }
    public class RfqCustomerSupportUpdateReleaseDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }
        public string RFQNumber { get; set; }
 
        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; } 
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }

        [Required]
        public string Unit { get; set; }
       
        public List<RfqCustomerSupportItemUpdateReleaseDto>? RfqCustomerSupportItems { get; set; }
        public List<RfqCustomerSupportNotesUpdateDto>? RfqCustomerSupportNotes { get; set; }

    } 
}
