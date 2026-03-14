using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EmployeeDetails
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int Emp_id { get; set; }

        public string Emp_name { get; set; }

        public decimal Emp_salary { get; set; }

        public string Emp_description { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? LastmodifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}
