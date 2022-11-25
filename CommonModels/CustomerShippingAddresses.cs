using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CustomerShippingAddresses
    {
        public int Id { get; set; }

        public string? ShippingAddress { get; set; }

        //public string? GSTNNumber { get; set; }

        public string? GooglePinLocation { get; set; }

        public bool SameasAddress { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int CustomerMasterId { get; set; }

        public CustomerMaster? CustomerMaster { get; set; }
    }
}
