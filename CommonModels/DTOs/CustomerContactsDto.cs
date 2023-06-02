using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CustomerContactsDto
    {
        public int Id { get; set; }

        public string? Salutation { get; set; }

        public string? CallName { get; set; }

        public string? MobileNumber { get; set; }
        public string? MobileCountryCode { get; set; }


        public string? LandLine { get; set; }

        public string? TimeToCall { get; set; }

        public bool Primary { get; set; } = true;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? AlternateMobileNumber { get; set; }
        public string? AlternateMobileCountryCode { get; set; }

        public bool IsActive { get; set; } = true;
       
        public string? Email { get; set; }

        public string? Extension { get; set; }

        public string? Language { get; set; }
        public string? SkypeId { get; set; }
        
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
       
    }
    public class CustomerContactsDtoPost
    {
        public string? Salutation { get; set; }

        public string? CallName { get; set; }

        public string? MobileNumber { get; set; }
        public string? MobileCountryCode { get; set; }


        public string? LandLine { get; set; }

        public string? TimeToCall { get; set; }

        public bool Primary { get; set; } = true;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? AlternateMobileNumber { get; set; }
        public string? AlternateMobileCountryCode { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Email { get; set; }

        public string? Extension { get; set; }
        public string? SkypeId { get; set; }
        public string? Language { get; set; }
       



    }
    public class CustomerContactsDtoUpdate
    {
        public int Id { get; set; }

        public string? Salutation { get; set; }

        public string? CallName { get; set; }

        public string? MobileNumber { get; set; }
        public string? MobileCountryCode { get; set; }

        public string? LandLine { get; set; }

        public string? TimeToCall { get; set; }

        public bool Primary { get; set; } = true;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? AlternateMobileNumber { get; set; }
        public string? AlternateMobileCountryCode { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Email { get; set; }

        public string? Extension { get; set; }
        public string? SkypeId { get; set; }
        public string? Language { get; set; }
        



    }
}
