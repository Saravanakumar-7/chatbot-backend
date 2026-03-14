using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EmployeeDetailsDto
    {
        [Required]
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

    public class EmployeeDetailsPostDto
    {
        [Required(ErrorMessage ="Emp_id is required")]
        public int Emp_id { get; set; }

        [Required]
        public string Emp_name { get; set; }

        public decimal Emp_salary { get; set; }

        public string Emp_description { get; set; }

        public string Unit { get; set; }
    }

    public class EmployeeDetailsUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Emp_id is required")]
        public int Emp_id { get; set; }
        public string Emp_name { get; set; }

        public decimal Emp_salary { get; set; }

        public string Emp_description { get; set; }

        public string Unit { get; set; }
    }
}
