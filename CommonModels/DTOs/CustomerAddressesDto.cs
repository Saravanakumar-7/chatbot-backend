using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CustomerAddressesDto
    {
        public int Id { get; set; }

        public string? InvoiceAddress { get; set; }

        public string? GSTNNumber { get; set; }

        public string? PANNumber { get; set; }
        public bool SameAsAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class CustomerAddressesDtoPost
    {

        public string? InvoiceAddress { get; set; }

        public string? GSTNNumber { get; set; }

        public string? PANNumber { get; set; }

        public bool SameAsAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;
      



    }
    public class CustomerAddressesDtoUpdate
    {
        public int Id { get; set; }

        public string? InvoiceAddress { get; set; }

        public string? GSTNNumber { get; set; }

        public string? PANNumber { get; set; }
        public bool SameAsAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;
        


    }
}
