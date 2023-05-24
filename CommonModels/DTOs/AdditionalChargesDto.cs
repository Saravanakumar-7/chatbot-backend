using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class AdditionalChargesDto
    {
        public int? Id { get; set; }

        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }
        public int? AddtionalChargesValueAmount { get; set; }
        public int? IGST { get; set; }
        public int? CGST { get; set; }
        public int? UTGST { get; set; }
        public int? SGST { get; set; }

        public bool ActiveStatus { get; set; } = true;

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class AdditionalChargesPostDto
    {
        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }
        public int? AddtionalChargesValueAmount { get; set; }

        public bool ActiveStatus { get; set; } = true;

        public int? IGST { get; set; }
        public int? CGST { get; set; }
        public int? UTGST { get; set; }
        public int? SGST { get; set; }
    }
    public class AdditionalChargesUpdateDto
    {
        public int? Id { get; set; }

        public string? AdditionalChargesLabelName { get; set; }
        public string? AddtionalChargesValueType { get; set; }
        public int? AddtionalChargesValueAmount { get; set; }
        public int? IGST { get; set; }
        public int? CGST { get; set; }

        public bool ActiveStatus { get; set; } = true;

        public int? UTGST { get; set; }
        public int? SGST { get; set; }

        public string Unit { get; set; }
    }


}
    
