using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VendorAddressDto
    {
        public int Id { get; set; }

        public string? POAddress { get; set; }

        public string? GSTNNumber { get; set; }

        public string? PANNumber { get; set; }

        public bool SameAsAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }


    public class VendorAddressPostDto
    {
        [StringLength(500, ErrorMessage = "POAddress can't be longer than 500 characters")]
        public string? POAddress { get; set; }
        public string? GSTNNumber { get; set; }
        [StringLength(500, ErrorMessage = "PANNumber can't be longer than 500 characters")]
        public string? PANNumber { get; set; }
        public bool SameAsAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;
       

    }

    public class VendorAddressUpdateDto
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessage = "POAddress can't be longer than 500 characters")]
        public string? POAddress { get; set; }
        public string? GSTNNumber { get; set; }
        [StringLength(500, ErrorMessage = "PANNumber can't be longer than 500 characters")]
        public string? PANNumber { get; set; }
        public bool SameAsAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;

    }

}
