using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqDto
    {   
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<RfqCustomerSupport>? rfqCustomerSupports { get; set; }
        //public List<RfqNotes>? rfqNotes { get; set; }
 
    }
    public class RfqPostDto
    { 
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public List<RfqCustomerSupport>? rfqCustomerSupports { get; set; }
        //public List<RfqNotes>? rfqNotes { get; set; }
    }
    public class RfqUpdateDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]
        public string? CustomerName { get; set; }

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public List<RfqCustomerSupport>? rfqCustomerSupports { get; set; }
        //public List<RfqNotes>? rfqNotes { get; set; }
    }

}
