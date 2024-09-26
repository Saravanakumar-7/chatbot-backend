using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class OtherChargesDto
    {
        [Key]
        public int? Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OtherChargesPostDto
    {
        public string? OtherChargesName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
       
    }
    public class OtherChargesUpdateDto
    {
        public int? Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
        public string Unit { get; set; }

    }
}