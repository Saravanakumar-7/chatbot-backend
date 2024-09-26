using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CustomerRelatedCustomer
    {
        [Key]
        public int Id { get; set; }
        public string? RelatedCustomerName { get; set; }
        public string? RelatedCustomerAlias { get; set; }
        public string? NatureOfRelationship { get; set; }
        public int CustomerMasterId { get; set; }
        public CustomerMaster? CustomerMaster { get; set; }
    }
}
