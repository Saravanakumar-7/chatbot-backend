using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities
{
    public class Architectures
    {
        public int Id { get; set; }
        public string? ArchitectName { get; set; }
        public int? PhoneNumber { get; set; }
        public string? EmailId { get; set; }
        public string? FirmName { get; set; }
        public string? SalesPerson { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? MarraigeAnniversery { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}