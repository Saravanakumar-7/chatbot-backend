using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entities
{
    public class VendorBanking
    {
        public int Id { get; set; }

        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? IBANCode { get; set; }
        public bool Primary { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public int VendorId { get; set; }

        public VendorMaster? VendorMaster { get; set; }

    }
}
