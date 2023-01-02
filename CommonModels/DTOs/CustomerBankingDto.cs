using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CustomerBankingDto
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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

         

    }
    public class CustomerBankingDtoPost
    {
        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? IBANCode { get; set; }
        public bool Primary { get; set; } = true;
        public bool IsActive { get; set; } = true;
      
         


    }
    public class CustomerBankingDtoUpdate
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

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }



    }
}
