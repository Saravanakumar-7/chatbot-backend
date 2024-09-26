using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ArchitectureDto
    {
        public int Id { get; set; }
        public string? ArchitectName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{1,12})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? FirmName { get; set; }
        public string? SalesPerson { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime? MarriageAnniversery { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ArchitecturePostDto
    {
        public string? ArchitectName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{1,12})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? FirmName { get; set; }
        public string? SalesPerson { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime? MarriageAnniversery { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }
    }
    public class ArchitectureUpdateDto
    {
        public int Id { get; set; }
        public string? ArchitectName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{1,12})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? FirmName { get; set; }
        public string? SalesPerson { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime? MarriageAnniversery { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }
    }


}
