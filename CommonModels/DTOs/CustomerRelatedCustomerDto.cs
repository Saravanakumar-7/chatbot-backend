using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CustomerRelatedCustomerDto
    {
        [Key]
        public int Id { get; set; }
        public string? RelatedCustomerName { get; set; }

        public string? RelatedCustomerAlias { get; set; }

        public string? NatureOfRelationship { get; set; }
    }
    public class CustomerRelatedCustomerPostDto {
        public string? RelatedCustomerName { get; set; }

        public string? RelatedCustomerAlias { get; set; }

        public string? NatureOfRelationship { get; set; }
    }
    public class CustomerRelatedCustomerUpdateDto
    {
        public int Id { get; set; }
        public string? RelatedCustomerName { get; set; }

        public string? RelatedCustomerAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

    }
}
