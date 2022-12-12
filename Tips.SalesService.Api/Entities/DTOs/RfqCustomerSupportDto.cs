using Entities.DTOs;
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
        public string? RevisionNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<RfqCustomerSupportItemDto>? rfqCustomerSupportItems { get; set; }

        public List<RfqCustomerSupportNotesDto>? rfqCustomerSupportNotes { get; set; }

        //public List<RfqNotes>? rfqNotes { get; set; }

    }
    public class RfqCustomerSupportPostDto
    { 
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }
        public string RFQNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public string? RevisionNumber { get; set; }

        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqCustomerSupportItemPostDto>? rfqCustomerSupportItems { get; set; }
        public List<RfqCustomerSupportNotesPostDto>? rfqCustomerSupportNotes { get; set; }

        //public List<RfqNotes>? rfqNotes { get; set; }
    }
    public class RfqCustomerSupportUpdateDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }
        public string RFQNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public string? RevisionNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqCustomerSupportItemUpdateDto>? rfqCustomerSupportItems { get; set; }
        public List<RfqCustomerSupportNotesUpdateDto>? rfqCustomerSupportNotes { get; set; }

        //public List<RfqNotes>? rfqNotes { get; set; }
    }
    public class RfqCustomerSupportUpdateReleaseDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }
        public string RFQNumber { get; set; }
 
        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public string? RevisionNumber { get; set; }
        public DateTime? RequestReceiveDate { get; set; }
        public DateTime? QuoteExpectDate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqCustomerSupportItemUpdateReleaseDto>? rfqCustomerSupportItems { get; set; }
        public List<RfqCustomerSupportNotesUpdateDto>? rfqCustomerSupportNotes { get; set; }

    } 
}
