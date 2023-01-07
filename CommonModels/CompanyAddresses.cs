using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CompanyAddresses
    {
        public int Id { get; set; }

        public string? POAddress { get; set; }

        public string? GSTNNumber { get; set; }

        public string? PANNumber { get; set; }

        public bool SameAsAddress { get; set; } = false;

        public bool IsActive { get; set; } = true;
  
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int CompanyMasterId { get; set; }

        public CompanyMaster? CompanyMaster { get; set; }
    }
}
