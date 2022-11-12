using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CompanyContactsDto
    {
        public int Id { get; set; }

        public string? Salutation { get; set; }

        public string? CallName { get; set; }

        public string? MobileNumber { get; set; }

        public string? LandLine { get; set; }

        public string? TimeToCall { get; set; }

        public bool Primary { get; set; } = true;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? AlternameMobileNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Email { get; set; }

        public string? Extension { get; set; }

        public string? Language { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class CompanyContactsDtoPost
    {
 
        public string? Salutation { get; set; }

        public string? CallName { get; set; }

        public string? MobileNumber { get; set; }

        public string? LandLine { get; set; }

        public string? TimeToCall { get; set; }

        public bool Primary { get; set; } = true;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? AlternameMobileNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Email { get; set; }

        public string? Extension { get; set; }

        public string? Language { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class CompanyContactsDtoUpdate
    {
        public int Id { get; set; }

        public string? Salutation { get; set; }

        public string? CallName { get; set; }

        public string? MobileNumber { get; set; }

        public string? LandLine { get; set; }

        public string? TimeToCall { get; set; }

        public bool Primary { get; set; } = true;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? AlternameMobileNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Email { get; set; }

        public string? Extension { get; set; }

        public string? Language { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
