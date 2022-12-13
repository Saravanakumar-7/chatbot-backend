using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReleaseCostingDto
    {
        public int Id { get; set; }
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }

        [Precision(13, 3)]
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ReleaseCostingPostDto
    { 
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }

        [Precision(13, 3)]
        public decimal ReleaseVersion { get; set; }
       
        [StringLength(100, ErrorMessage = "ReleaseNote can't be longer than 100 characters")]
        public string ReleaseNote { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
