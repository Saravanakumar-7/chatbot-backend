using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class RoleAccess
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
        public bool ApprovalI { get; set; }
        
        [DefaultValue(false)]
        public bool ApprovalII { get; set; }
         
        
        public string? LastModifiedBy { get; set; }
        
        public DateTime? LastModifiedOn { get; set; }

        public int RoleId { get; set; }
        public Role? Roles { get; set; }
    }
}
