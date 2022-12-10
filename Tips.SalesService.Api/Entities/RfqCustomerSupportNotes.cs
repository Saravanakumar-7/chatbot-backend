using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class RfqCustomerSupportNotes
    {
        public int Id { get; set; }
        public string? CustomerSupportCategory { get; set; } 
         public string? CustomerSupportNotes { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqCustomerSupportId { get; set; }
        public RfqCustomerSupport? rfqCustomerSupport { get; set; }

        //public int RfqId { get; set; }
        //public Rfq? Rfq { get; set; }
    }
}
