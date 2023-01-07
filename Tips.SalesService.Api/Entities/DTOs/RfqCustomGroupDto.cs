using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqCustomGroupDto
    {
        public int Id { get; set; }
        public string? CustomGroupName { get; set; }
        public string? Remark { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class RfqCustomGroupPostDto
    {
        [Required(ErrorMessage = "CustomGroupName is required")]
        [StringLength(500, ErrorMessage = "CustomGroupName can't be longer than 500 characters")]
        public string? CustomGroupName { get; set; }
        public string? Remark { get; set; }
       
    }
    public class RfqCustomGroupUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CustomGroupName is required")]
        [StringLength(500, ErrorMessage = "CustomGroupName can't be longer than 500 characters")]
        public string? CustomGroupName { get; set; }
        public string? Remark { get; set; }
        public string Unit { get; set; }
       
    }
}
