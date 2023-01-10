using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class ForeCastCustomerSupportNotes
    {

        public int Id { get; set; }
        public string? CustomerSupportCategory { get; set; }
        public string? CustomerSupportNotes { get; set; }

        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int ForeCastCustomerSupportId { get; set; }
        public ForeCastCustomerSupport? ForeCastCustomerSupport { get; set; }
    }
}
