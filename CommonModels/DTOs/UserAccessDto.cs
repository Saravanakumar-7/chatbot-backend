using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class UserAccessDto
    {
        [Key]
        public int Id { get; set; }
        public string? Menu { get; set; }
        public string? FormName { get; set; }

        [DefaultValue(false)]
        public bool Create { get; set; }

        [DefaultValue(false)]
        public bool Delete { get; set; }

        [DefaultValue(false)]
        public bool View { get; set; }

        [DefaultValue(false)]
        public bool Edit { get; set; }

        [DefaultValue(false)]
        public bool Download { get; set; }

        [DefaultValue(false)]
        public bool Upload { get; set; } 

        [DefaultValue(false)]
        public bool Confirm { get; set; }

        [DefaultValue(false)]
        public bool ApprovalI { get; set; }

        [DefaultValue(false)]
        public bool ApprovalII { get; set; }

        [DefaultValue(false)]
        public bool Print { get; set; }
        public int UserId { get; set; }
        [DefaultValue(false)]
        public bool Table { get; set; }
        public string? UserName { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class UserAccessPostDto
    {
        public string? Menu { get; set; }
        public string? FormName { get; set; }

        [DefaultValue(false)]
        public bool Create { get; set; }

        [DefaultValue(false)]
        public bool Delete { get; set; }

        [DefaultValue(false)]
        public bool View { get; set; }

        [DefaultValue(false)]
        public bool Edit { get; set; }

        [DefaultValue(false)]
        public bool Download { get; set; }
        [DefaultValue(false)]
        public bool Upload { get; set; }

        [DefaultValue(false)]
        public bool Confirm { get; set; }

        [DefaultValue(false)]
        public bool ApprovalI { get; set; }

        [DefaultValue(false)]
        public bool ApprovalII { get; set; }
        [DefaultValue(false)]
        public bool Print { get; set; }
        [DefaultValue(false)]
        public bool Table { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }

    }
    public class UserAccessUpdateDto
    {
        [Key]
        public int Id { get; set; }
        public string? Menu { get; set; }
        public string? FormName { get; set; }

        [DefaultValue(false)]
        public bool Create { get; set; }

        [DefaultValue(false)]
        public bool Delete { get; set; }

        [DefaultValue(false)]
        public bool View { get; set; }

        [DefaultValue(false)]
        public bool Edit { get; set; }

        [DefaultValue(false)]
        public bool Download { get; set; }
        [DefaultValue(false)]
        public bool Upload { get; set; }

        [DefaultValue(false)]
        public bool Confirm { get; set; }

        [DefaultValue(false)]
        public bool ApprovalI { get; set; }

        [DefaultValue(false)]
        public bool Table { get; set; }

        [DefaultValue(false)]
        public bool ApprovalII { get; set; }
        [DefaultValue(false)]
        public bool Print { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
