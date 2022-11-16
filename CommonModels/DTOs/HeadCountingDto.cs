using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class HeadCountingDto
    {

        public int Id { get; set; }

        public string? DepartmentSkill { get; set; }

        public string? NumberOfPeople { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class HeadCountingPostDto
    {
        [StringLength(500, ErrorMessage = "DepartmentSkill can't be longer than 500 characters")]

        public string? DepartmentSkill { get; set; }

        [StringLength(500, ErrorMessage = "NumberOfPeople can't be longer than 500 characters")]

        public string? NumberOfPeople { get; set; }
    }

    public class HeadCountingUpdateDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "DepartmentSkill can't be longer than 500 characters")]
        public string? DepartmentSkill { get; set; }

        [StringLength(500, ErrorMessage = "NumberOfPeople can't be longer than 500 characters")]

        public string? NumberOfPeople { get; set; }
    }
}
