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
    public class Rfq
    {
        
        public int Id { get; set; }
        public string? CustomerName { get; set; }

        public int RfqNumber { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public float? RevNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<RfqCustomerSupport>? rfqCustomerSupports { get; set; }
        public List<RfqNotes>? rfqNotes { get; set; }


    }
}
