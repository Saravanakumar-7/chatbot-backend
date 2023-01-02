using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CustomerShippingAddressesDto
    {
        public int Id { get; set; }

        public string? ShippingAddress { get; set; }

        public string? GooglePinLocation { get; set; }

        public bool SameasAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        
    }
    public class CustomerShippingAddressesDtoPost
    {

        public string? ShippingAddress { get; set; }

        public string? GooglePinLocation { get; set; }

        public bool SameasAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;


    }
    public class CustomerShippingAddressesDtoUpdate
    {
        public int Id { get; set; }

        public string? ShippingAddress { get; set; }

        public string? GooglePinLocation { get; set; }

        public bool SameasAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }

    }
}
