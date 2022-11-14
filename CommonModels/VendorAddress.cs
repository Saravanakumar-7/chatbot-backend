using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class VendorAddress
    {
        public int Id { get; set; }

        public string? PoAddress { get; set; }

        public string? GSTNNumber { get; set; }

        public string? PANNumber { get; set; }

        public bool SameasAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int VendorMasterId { get; set; }
        public VendorMaster? VendorMaster { get; set; }
    }
}
