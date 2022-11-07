using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VendorMasterBankingDto
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
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class VendorMasterBankingPostDto
    {
        [StringLength(100, ErrorMessage = "BankName can't be longer than 100 characters")]
        public string? BankName { get; set; }
        [StringLength(100, ErrorMessage = "Branch can't be longer than 100 characters")]
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? IBANCode { get; set; }
        public bool Primary { get; set; } = true;
        public bool IsActive { get; set; } = true;

    }

    public class VendorMasterBankingUpdateDto
    {
        public int Id { get; set; }
        [StringLength(100, ErrorMessage = "BankName can't be longer than 100 characters")]
        public string? BankName { get; set; }
        [StringLength(100, ErrorMessage = "Branch can't be longer than 100 characters")]
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? IBANCode { get; set; }
        public bool Primary { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}
